using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterCardTradingGame.DataBase.Repositories
{
    internal class UserRepository
    {
        //DBAcces initialization
        private DBAccess _dbAccess { get; set; }

        //Constructor
        public UserRepository(string connectionString)
        {
            _dbAccess = new DBAccess(connectionString);
        }

        //Help functions
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int GetNewDeckId()
        {
            int newId = 0;
            _dbAccess.ExecuteQuery(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT COALESCE(MAX(deck_id), 0) + 1 FROM users;", conn))
                {
                    newId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                return newId;
            });
            return newId;
        }

        private int GetNewStackId()
        {
            int newId = 0;
            _dbAccess.ExecuteQuery(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT COALESCE(MAX(stack_id), 0) + 1 FROM users;", conn))
                {
                    newId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                return newId;
            });
            return newId;
        }

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
        internal void AddUser(string username, string password)
        {
            try
            {
                int deckId = GetNewDeckId();
                int stackId = GetNewStackId();

                
                _dbAccess.ExecuteQuery<int>(conn =>
                {
                    using (var cmd = new NpgsqlCommand(
                               "INSERT INTO users (username, password,level, coins, deck_id, stack_id) VALUES (@username, @password,@level,@coins, @deckId, @stackId);",
                               conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@deckId", deckId);
                        cmd.Parameters.AddWithValue("@stackId", stackId);
                        cmd.Parameters.AddWithValue("@level", 0);
                        cmd.Parameters.AddWithValue("@coins", 0);

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
        internal void UpdateUser(string newUsername, string currentUsername, string newPassword)
        {
            try
            {
                _dbAccess.ExecuteQuery<int>(conn =>
                {
                    using (var cmd = new NpgsqlCommand(
                               "UPDATE users SET username = @newUsername, password = @password WHERE username = @currentUsername;",
                               conn))
                    {
                        // Binden der Parameter an den Befehl
                        cmd.Parameters.AddWithValue("@newUsername", newUsername); // Der neue Benutzername
                        cmd.Parameters.AddWithValue("@password", newPassword); // Das neue Passwort
                        cmd.Parameters.AddWithValue("@currentUsername", currentUsername); // Der aktuelle Benutzername
                        // Führen Sie den Befehl aus
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
                    using (var cmd = new NpgsqlCommand(
                               "SELECT u.username, u.coins, u.level, c.card_id, c.name, c.element, c.damage FROM users u JOIN decks d ON u.deck_id = d.deck_id JOIN cards c ON d.card_id = c.card_id WHERE u.username = @username\r\n",
                               conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        using (var reader = cmd.ExecuteReader())
                        {
                            var result = new Dictionary<string, object>();
                            bool userDataSet = false;
                            while (reader.Read())
                            {
                                if (!userDataSet)
                                {
                                    result.Add("username", reader["username"]);
                                    result.Add("coins", reader["coins"]);
                                    result.Add("level", reader["level"]);
                                    userDataSet = true;
                                }

                                var cardData = new Dictionary<string, object>();
                                cardData.Add("card_id", reader["card_id"]);
                                cardData.Add("name", reader["name"]);
                                cardData.Add("element", reader["element"]);
                                cardData.Add("damage", reader["damage"]);
                                result.Add($"card_{reader["card_id"]}", cardData);
                            }

                            return result;
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
        //get stack_id from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int getStackIsByUserId(int userId)
        {
            try
            {
                return _dbAccess.ExecuteQuery<int>(conn =>
                {
                    using (var cmd = new NpgsqlCommand("SELECT stack_id FROM users WHERE user_id = @user_Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
        
        //Read user_stats from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}


