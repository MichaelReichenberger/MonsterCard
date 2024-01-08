using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MonsterCardTradingGame.Models;
using Newtonsoft.Json;
using Npgsql;
using static System.Net.Mime.MediaTypeNames;

namespace MonsterCardTradingGame.DataBase.Repositories
{
    public class UserRepository : IRepository
    {
        
        private DBAccess _dbAccess { get; set; }
        
        
        public UserRepository(string connectionString)
        {
            _dbAccess = new DBAccess(connectionString);
            
        }

        public int GetFirstId()
        {
            return _dbAccess.ExecuteQuery(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT MIN(user_id) FROM users;", conn))
                {
                    return (int)cmd.ExecuteScalar();
                }
            });
        }

        public int GetNextId()
        {
            return _dbAccess.ExecuteQuery<int>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT MAX(user_id) FROM users", conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result == DBNull.Value)
                    {
                        return 1;
                    }
                    else
                    {
                        return Convert.ToInt32(result) + 1;
                    }
                }
            });
        }

        public void DeleteById(int id)
        {
            _dbAccess.ExecuteQuery<int>(conn =>
            {
                using (var cmd = new NpgsqlCommand("DELETE FROM users WHERE user_id = @userId", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", id);
                    return cmd.ExecuteNonQuery();
                }
            });
        }

        //Help functions
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int? GetUserId(string username)
        {
            return _dbAccess.ExecuteQuery(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT u.user_id FROM users u WHERE u.username = @username;", conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    return (int?)cmd.ExecuteScalar();
                }
            });
        }

        public int GetUserIdForTest(string username)
        {
            return _dbAccess.ExecuteQuery(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT u.user_id FROM users u WHERE u.username = @username;", conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    return (int)cmd.ExecuteScalar();
                }
            });
        }

        public virtual string GetPasswordByUsername(string username)
        {
            return _dbAccess.ExecuteQuery(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT password FROM users WHERE username=@username;", conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    return Convert.ToString(cmd.ExecuteScalar());
                }
            });
        }

        

        internal string GetUsername(int userId)
        {
            return _dbAccess.ExecuteQuery(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT username FROM users WHERE user_id=@userId;", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return Convert.ToString(cmd.ExecuteScalar());
                }
            });
        }

        internal int GetCoins(int userId)
        {
            return _dbAccess.ExecuteQuery(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT coins FROM users WHERE user_id=@userId;", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            });
        }

        public void SubstractCoins(int userId, int coins)
        {
            _dbAccess.ExecuteTransaction((conn, trans) =>
            {
                try
                {
                    using (var cmd = new NpgsqlCommand("UPDATE users SET coins = coins - @coins WHERE user_id = @userId;",
                               conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@coins", coins);
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback(); 
                    throw new Exception("Error while adding user.");
                }
            });
        }

        public void InitialiseStatsEmpty(int userId, string username)
        {
            _dbAccess.ExecuteTransaction((conn, transaction) =>
            {
                try
                {
                    using (var cmd = new NpgsqlCommand(
                               "INSERT INTO user_stats (user_id, username, elo, wins, losses, games_played, win_loose_ratio) VALUES (@userId, @username, @elo, @wins, @losses, @gamesPlayed, @winLooseRatio)",
                               conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@elo", 100);
                        cmd.Parameters.AddWithValue("@wins", 0);
                        cmd.Parameters.AddWithValue("@losses", 0);
                        cmd.Parameters.AddWithValue("@gamesPlayed", 0);
                        cmd.Parameters.AddWithValue("@winLooseRatio", 0);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("Error while inserting stats into DB");
                }
            });
        }

        //Add user to DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void AddUser(string username, string password, string image, string bio, string name)
        {
            _dbAccess.ExecuteTransaction((conn, trans) =>
            {
                try
                {
                    using (var cmd = new NpgsqlCommand(
                               "INSERT INTO users (username, password, level, coins, bio, image, role, name) VALUES (@username, @password, @level, @coins, @bio, @image, @role, @name);",
                               conn))
                    {
                        
                        cmd.Transaction = trans;
                        image = String.IsNullOrEmpty(image) ? "-" : image;
                        bio = String.IsNullOrEmpty(bio) ? "-" : bio;
                        name = String.IsNullOrEmpty(name) ? "-" : name;

                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@bio", bio);
                        cmd.Parameters.AddWithValue("@image", image);
                        cmd.Parameters.AddWithValue("@level", 0);
                        cmd.Parameters.AddWithValue("@coins", 20);
                        cmd.Parameters.AddWithValue("@name", name);
                        if (username == "admin")
                        {
                            cmd.Parameters.AddWithValue("@role", "admin");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@role", "user");
                        }
                        
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit(); 
                    int userId = GetUserId(username).Value;
                    if (userId != null)
                    { 
                        InitialiseStatsEmpty(userId, username);
                    }
                }
                catch (Exception e)
                {
                    trans.Rollback(); 
                    throw new Exception("User with same username already registered");
                }
            });
        }




        //Update user in DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UpdateUser(string newName, string currentUsername, string newPassword, string image, string bio)
        {
            _dbAccess.ExecuteTransaction((conn, trans) =>
            {
                try
                {
                   
                    var updateQuery = new StringBuilder("UPDATE users SET ");
                    if (!string.IsNullOrEmpty(newName))
                    {
                        updateQuery.Append("name = @newName, ");
                    }
                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        updateQuery.Append("password = @password, ");
                    }
                    if (!string.IsNullOrEmpty(bio))
                    {
                        updateQuery.Append("bio=@bio, ");
                    }
                    if (!string.IsNullOrEmpty(image))
                    {
                        updateQuery.Append("image=@image, ");
                    }

                   
                    if (updateQuery[updateQuery.Length - 2] == ',')
                    {
                        updateQuery.Remove(updateQuery.Length - 2, 2);
                    }
                    updateQuery.Append(" WHERE username = @currentUsername;");

                    using (var cmd = new NpgsqlCommand(updateQuery.ToString(), conn))
                    {

                        cmd.Transaction = trans;


                        if (!string.IsNullOrEmpty(newName))
                        {
                            cmd.Parameters.AddWithValue("@newName", newName);
                        }
                        if (!string.IsNullOrEmpty(newPassword))
                        {
                            cmd.Parameters.AddWithValue("@password", newPassword);
                        }
                        cmd.Parameters.AddWithValue("@currentUsername", currentUsername);
                        if (!string.IsNullOrEmpty(bio))
                        {
                            cmd.Parameters.AddWithValue("@bio", bio);
                        }
                        if (!string.IsNullOrEmpty(image))
                        {
                            cmd.Parameters.AddWithValue("@image", image);
                        }


                        cmd.ExecuteNonQuery();
                    }

                    if (!string.IsNullOrEmpty(newName))
                    {
                        using (var cmd = new NpgsqlCommand("UPDATE user_stats SET username = @newUsername WHERE username = @currentUsername", conn))
                        {
                            cmd.Transaction = trans;
                            cmd.Parameters.AddWithValue("@newUsername", newName);
                            cmd.Parameters.AddWithValue("@currentUsername", currentUsername);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    trans.Commit(); 
                }
                catch (Exception e)
                {
                    trans.Rollback(); 
                    throw new Exception("Error while updating user", e);
                }
            });
        }


        //Get userdata from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public User GetUserData(int userId)
        {
            try
            {
                return _dbAccess.ExecuteQuery(conn =>
                {
                    using var cmd = new NpgsqlCommand("SELECT * FROM users WHERE user_id = @userId;", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        var user = new User();
                        user.Username = reader.GetString(reader.GetOrdinal("username"));
                        user.Name = reader.GetString(reader.GetOrdinal("Name"));
                        user.Coins = reader.GetInt32(reader.GetOrdinal("coins"));
                        user.Level = reader.GetInt32(reader.GetOrdinal("level"));
                        user.UserId = reader.GetInt32(reader.GetOrdinal("user_id"));
                        user.Password = reader.GetString(reader.GetOrdinal("password"));
                        user.Image = reader.GetString(reader.GetOrdinal("image"));
                        user.Bio = reader.GetString(reader.GetOrdinal("bio"));
                        return user;
                    }
                    return null;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Error while getting user data", e); // Added original exception to the throw statement for better debugging
            }
        }

        public virtual User GetUserCredentials(string username)
        {
            try
            {
                return _dbAccess.ExecuteQuery(conn =>
                {
                    using var cmd = new NpgsqlCommand("SELECT * FROM users WHERE username = @username;", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        var user = new User();
                        user.Username = reader.GetString(reader.GetOrdinal("username")); // Changed from "name" to "username"
                        user.Password = reader.GetString(reader.GetOrdinal("password"));
                        return user;
                    }
                    return null;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Error while getting user data", e); // Added original exception to the throw statement for better debugging
            }
        }

        public virtual string GetRole(int userId)
        {
            return _dbAccess.ExecuteQuery(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT role FROM users WHERE user_id = @userId;", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return Convert.ToString(cmd.ExecuteScalar());
                }
            });
        }
    }
}


   