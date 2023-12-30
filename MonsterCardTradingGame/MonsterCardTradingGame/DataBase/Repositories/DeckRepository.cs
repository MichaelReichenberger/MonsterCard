using MonsterCardTradingGame.BusinessLogic;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.DataBase.Repositories
{
    public class DeckRepository : IRepository
    {

        private DBAccess _dbAccess { get; set; }
        private Parser _parser;
        private UserRepository _userRepository;

        public DeckRepository(string connectionString)
        {
            _dbAccess = new DBAccess(connectionString);
            _userRepository = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _parser = new Parser();
        }

        public int GetFirstId()
        {
            throw new NotImplementedException();
        }

        public int GetNextId()
        {
            throw new NotImplementedException();
        }

        public void DeleteById(int id)
        {
            _dbAccess.ExecuteQuery<int>(conn =>
            {
                using (var cmd = new NpgsqlCommand("DELETE FROM decks WHERE user_id = @userId", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", id);
                    return cmd.ExecuteNonQuery();
                }
            });
        }


    //Insert deck in DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void InsertCardsIntoDeck(int userId, List<string> cardIds)
        {
            foreach (var CardId in cardIds)
            {
                try
                {
                    _dbAccess.ExecuteQuery<int>(conn =>
                    {
                        using (var cmd =
                               new NpgsqlCommand(
                                   "INSERT INTO decks (user_id, unique_id) VALUES (@userId, @cardId)",
                                   conn))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId);
                            cmd.Parameters.AddWithValue("@cardId", CardId);
                            return cmd.ExecuteNonQuery();
                        }
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw new Exception("Error inserting card into deck");
                }
            }
        }

        //Get deck from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetDeckByUserId(int userId)
        {
            try
            {
                return _dbAccess.ExecuteQuery<string>(conn =>
                {
                    using (var cmd = new NpgsqlCommand("SELECT unique_id FROM decks WHERE user_id = @userId", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            var result = new StringBuilder();
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    result.Append($"{reader.GetName(i)}: {reader[i].ToString()}, ");
                                }
                                result.AppendLine();
                            }
                            return result.ToString();
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Error getting deck by userId");
            }
            
        }

        public void DeleteDeckByUserId(int userId)
        {
            try
            {
                _dbAccess.ExecuteQuery<int>(conn =>
                {
                    using (var cmd = new NpgsqlCommand("DELETE FROM decks WHERE user_id = @userId", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        return cmd.ExecuteNonQuery();
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Error while removing deck");
            }
        }

        public int CountCardsInDeck(int userId)
        {
            try
            {
                return _dbAccess.ExecuteQuery<int>(conn =>
                {
                    using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM decks WHERE user_id = @userId", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        // Safely convert the result to int
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Error while getting deck count");
            }
        }
    }
}
