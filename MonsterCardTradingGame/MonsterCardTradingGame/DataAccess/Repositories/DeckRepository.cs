using MonsterCardTradingGame.BusinessLogic;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Models;
using System.Xml;

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
            _dbAccess.ExecuteTransaction((conn, transaction) =>
            {
                try
                {
                    using (var cmd = new NpgsqlCommand("DELETE FROM decks WHERE user_id = @userId", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", id);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("Error while removing deck");
                }
            });
        }


        //Insert deck in DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void InsertCardsIntoDeck(int userId, List<string> cardIds)
        {
            _dbAccess.ExecuteTransaction((conn, transaction) =>
            {
                try
                {
                    foreach (var CardId in cardIds)
                    {
                        using (var cmd = new NpgsqlCommand(
                                   "INSERT INTO decks (user_id, unique_id) VALUES (@userId, @cardId)",
                                   conn))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId);
                            cmd.Parameters.AddWithValue("@cardId", CardId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("Error inserting card into deck");
                }
            });
        }

        //Get deck from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<Card> GetDeckByUserId(int userId)
        {
            try
            {
                return _dbAccess.ExecuteQuery<List<Card>>(conn =>
                {
                    using (var cmd = new NpgsqlCommand("SELECT * FROM card_stacks WHERE unique_id IN (SELECT unique_id FROM decks WHERE user_id = @userId)", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            List<Card> cards = new List<Card>();
                            while (reader.Read())
                            {
                                string name = reader.GetString(reader.GetOrdinal("name"));
                                string elementString = reader.GetString(reader.GetOrdinal("element"));
                                double damage = reader.GetDouble(reader.GetOrdinal("damage"));
                                string uniqueId = reader.GetString(reader.GetOrdinal("unique_id"));
                                // Convert the string to the Element enum
                                GameManager.Element element;
                                if (!Enum.TryParse<GameManager.Element>(elementString, true, out element))
                                {
                                    // Handle the case where the element is not valid or throw an exception
                                    throw new ArgumentException("Invalid element value in database for card: " + uniqueId);
                                }
                                // Create and return the new Card object
                                Card card = new Card(name, element, damage, uniqueId);
                                cards.Add(card);
                            }
                            return cards;
                        }
                    }
                });
            }
            catch (Exception e)
            {
                throw new Exception("Error getting deck by userId", e);
            }
        }



        //Get UniqueId from deck by userId
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual string GetDeckUniqueIdsByUserId(int userId)
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
                                    result.Append($"{reader[i].ToString()}, ");
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
                throw new Exception("Error getting deck by userId");
            }
        }

        public void DeleteDeckByUserId(int userId)
        {
            _dbAccess.ExecuteTransaction((conn, transaction) =>
            {
                try
                {
                    using (var cmd = new NpgsqlCommand("DELETE FROM decks WHERE user_id = @userId", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("Error while removing deck");
                }
            });
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
                throw new Exception("Error while getting deck count");
            }
        }
    }
}
