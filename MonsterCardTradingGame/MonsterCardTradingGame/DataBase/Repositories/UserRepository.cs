using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterCardTradingGame.DataBase.Repositories
{
    internal class UserRepository : IRepository
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
                    // Überprüfen, ob das Ergebnis NULL ist (d.h. keine Einträge in der Tabelle)
                    if (result == DBNull.Value)
                    {
                        return 1; // Keine Einträge vorhanden, also 1 zurückgeben
                    }
                    else
                    {
                        return Convert.ToInt32(result) + 1; // MAX(package_id) + 1 zurückgeben
                    }
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

        //Add user to DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void AddUser(string username, string password, string image, string bio)
        {
            try
            {
                _dbAccess.ExecuteQuery<int>(conn =>
                {
                    using (var cmd = new NpgsqlCommand(
                               "INSERT INTO users (username, password,level, coins, bio, image) VALUES (@username, @password,@level,@coins, @bio, @image);",
                               conn))
                    {
                        if (String.IsNullOrEmpty(image))
                        {
                            image = "-";
                        }
                        if (String.IsNullOrEmpty(bio))
                        {
                            bio = "-";
                        }
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@bio", bio);
                        cmd.Parameters.AddWithValue("@image", image);
                        cmd.Parameters.AddWithValue("@level", 0);
                        cmd.Parameters.AddWithValue("@coins", 100);
                        return cmd.ExecuteNonQuery();
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
           
        }

        //Update user in DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void UpdateUser(string newUsername, string currentUsername, string newPassword, string image, string bio)
        {
            try
            {
                _dbAccess.ExecuteQuery<int>(conn =>
                {
                    using (var cmd = new NpgsqlCommand(
                               "UPDATE users SET " +
                               (string.IsNullOrEmpty(newUsername) ? "" : "username = @newUsername, ") +
                               (string.IsNullOrEmpty(newPassword) ? "" : "password = @password, ") +
                               (string.IsNullOrEmpty(bio) ? "" : "bio=@bio, ") +
                               (string.IsNullOrEmpty(image) ? "" : "image=@image, ") +
                               " WHERE username = @currentUsername;",
                               conn))
                    {
                        // Binden der Parameter an den Befehl
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
                        return cmd.ExecuteNonQuery();
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        //Get userdata from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal Dictionary<string, object> GetUserData(string username)
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
                        var result = new Dictionary<string, object>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            result.Add(reader.GetName(i), reader.GetValue(i));
                        }
                        return result;
                    }
                    return null;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
    }
}


