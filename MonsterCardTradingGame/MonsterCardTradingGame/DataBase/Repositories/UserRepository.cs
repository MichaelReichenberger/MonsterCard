using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MonsterCardTradingGame.Models;
using Newtonsoft.Json;
using Npgsql;

namespace MonsterCardTradingGame.DataBase.Repositories
{
    public class UserRepository : IRepository
    {
        //DBAcces initialization
        private DBAccess _dbAccess { get; set; }
        
        //Constructor
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
        internal int? GetUserId(string username)
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

        internal string getPasswordByUsername(string username)
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

        internal void SubstractCoins(int userId, int coins)
        {
            _dbAccess.ExecuteQuery<int>(conn =>
            {
                using (var cmd = new NpgsqlCommand("UPDATE users SET coins = coins - @coins WHERE user_id = @userId;",
                           conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@coins", coins);
                    return cmd.ExecuteNonQuery();
                }
            });
        }



        //Add user to DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void AddUser(string username, string password, string image, string bio)
        {
            _dbAccess.ExecuteTransaction((conn, trans) =>
            {
                try
                {
                    using (var cmd = new NpgsqlCommand(
                               "INSERT INTO users (username, password, level, coins, bio, image) VALUES (@username, @password, @level, @coins, @bio, @image);",
                               conn))
                    {
                        // Setting the transaction
                        cmd.Transaction = trans;

                        // Handle null or empty fields
                        image = String.IsNullOrEmpty(image) ? "-" : image;
                        bio = String.IsNullOrEmpty(bio) ? "-" : bio;

                        // Adding parameters
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@bio", bio);
                        cmd.Parameters.AddWithValue("@image", image);
                        cmd.Parameters.AddWithValue("@level", 0);
                        cmd.Parameters.AddWithValue("@coins", 80);

                        // Execute the command
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit(); // Commit the transaction after all operations
                }
                catch (Exception e)
                {
                    trans.Rollback(); // Rollback the transaction in case of an error
                    throw new Exception("Error while adding user.");
                }
            });
        }


        //Update user in DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void UpdateUser(string newUsername, string currentUsername, string newPassword, string image, string bio)
        {
            _dbAccess.ExecuteTransaction((conn, trans) =>
            {
                try
                {
                    var updateQuery = new StringBuilder("UPDATE users SET ");
                    if (!string.IsNullOrEmpty(newUsername))
                    {
                        updateQuery.Append("username = @newUsername, ");
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
                    // Remove the last comma and space
                    if (updateQuery[updateQuery.Length - 2] == ',')
                    {
                        updateQuery.Remove(updateQuery.Length - 2, 2);
                    }
                    updateQuery.Append(" WHERE username = @currentUsername;");

                    using (var cmd = new NpgsqlCommand(updateQuery.ToString(), conn))
                    {
                        // Setting the transaction
                        cmd.Transaction = trans;

                        // Adding parameters
                        if (!string.IsNullOrEmpty(newUsername))
                        {
                            cmd.Parameters.AddWithValue("@newUsername", newUsername);
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

                        // Execute the command
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit(); // Commit the transaction after all operations
                }
                catch (Exception e)
                {
                    trans.Rollback(); // Rollback the transaction in case of an error
                    throw new Exception("Error while updating user", e);
                }
            });
        }

        //Get userdata from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal User GetUserData(int userId)
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
                        user.Name = reader.GetString(reader.GetOrdinal("username")); // Changed from "name" to "username"
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


        internal User GetUserCredentials(string username)
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
    }
}


   