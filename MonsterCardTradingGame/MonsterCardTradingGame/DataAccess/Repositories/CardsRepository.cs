using MonsterCardTradingGame.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using MonsterCardTradingGame.Models;
using System.Xml.Linq;
using MonsterCardTradingGame.DataBase;
using MonsterCardTradingGame.DataBase.Repositories;
using static System.Net.Mime.MediaTypeNames;

namespace MonsterCardTradingGame.DataAccess.Repositories
{
    public class CardsRepository :IRepository
    {

        private DBAccess _dbAccess { get; set; }
        private Parser _parser;
        private UserRepository _userRepository;
        private TradingsRepository _tradingsRepository;
        public CardsRepository(string connectionString)
        {
            _dbAccess = new DBAccess(connectionString);
            _userRepository = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _tradingsRepository = new TradingsRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
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


        //Get list of users Cards from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual List<Card> GetCardsFromDB(int userId)
        {
            return _dbAccess.ExecuteQuery<List<Card>>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM card_stacks WHERE user_id = @userId", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        var cards = new List<Card>();
                        while (reader.Read())
                        {
                            string name = reader.GetString(reader.GetOrdinal("name"));
                            string elementString = reader.GetString(reader.GetOrdinal("element"));
                            double damage = reader.GetDouble(reader.GetOrdinal("damage"));
                            string uniqueId = reader.GetString(reader.GetOrdinal("unique_id"));
                            
                            GameManager.Element element;
                            if (!Enum.TryParse<GameManager.Element>(elementString, true, out element))
                            {
                                
                                throw new ArgumentException("Invalid element value in database for card: " + name);
                            }
                            
                            Card card = new Card(name, element, damage, uniqueId);
                            cards.Add(card);
                        }
                        return cards;
                    }
                }
            });
        }

        //Get List of all cards from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual List<Card> GetAllCardsFromDB()
        {
            return _dbAccess.ExecuteQuery<List<Card>>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM card_stacks", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        var cards = new List<Card>();
                        while (reader.Read())
                        {
                            string name = reader.GetString(reader.GetOrdinal("name"));
                            string elementString = reader.GetString(reader.GetOrdinal("element"));
                            double damage = reader.GetDouble(reader.GetOrdinal("damage"));
                            string uniqueId = reader.GetString(reader.GetOrdinal("unique_id"));

                            GameManager.Element element;
                            if (!Enum.TryParse<GameManager.Element>(elementString, true, out element))
                            {

                                throw new ArgumentException("Invalid element value in database for card: " + name);
                            }

                            Card card = new Card(name, element, damage, uniqueId);
                            cards.Add(card);
                        }
                        return cards;
                    }
                }
            });
        }


        //Get CardModel from DB, so that it can be used in trading/battle
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual Card GetCardModelFromDB(string uniqueId)
        {
            return _dbAccess.ExecuteQuery<Card>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM card_stacks WHERE unique_id = @uniqueId", conn))
                {
                    cmd.Parameters.AddWithValue("@uniqueId", uniqueId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string name = reader.GetString(reader.GetOrdinal("name"));
                            string elementString = reader.GetString(reader.GetOrdinal("element"));
                            double damage = reader.GetDouble(reader.GetOrdinal("damage"));
                            string uniqueId = reader.GetString(reader.GetOrdinal("unique_id"));
                            
                            GameManager.Element element;
                            if (!Enum.TryParse<GameManager.Element>(elementString, true, out element))
                            {
                                
                                throw new ArgumentException("Invalid element value in database for card: " + uniqueId);
                            }
                            
                            Card card = new Card(name, element, damage, uniqueId);
                            return card;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            });
        }


        //Check if a user own a card in case of a trade
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CheckIfUserOwnsCard(int userId, string cardId)
        {
            return _dbAccess.ExecuteQuery<bool>(conn =>
            {
                using (var cmd = new NpgsqlCommand(
                           "SELECT EXISTS(SELECT 1 FROM card_stacks WHERE user_id = @userId AND unique_id = @cardId)",
                           conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@cardId", cardId);
                    return (bool)cmd.ExecuteScalar();
                }
            });
        }

        //Transfer card after deal is accepted
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void TransferReceivedCard(string dealId, string receivedCard)
        {
            int userId = _tradingsRepository.GetSenderIdByDealId(dealId);
            _dbAccess.ExecuteTransaction((conn, transaction) =>
            {
                try
                {
                    using (var cmd = new NpgsqlCommand(
                               "UPDATE card_stacks SET user_id = @userId WHERE unique_id = @receivedCard", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@receivedCard", receivedCard);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("Error transferring received card");
                }
            });
        }

        //Transfer card after deal is accepted
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void TransferCard(string dealId, int userId)
        {
            Console.WriteLine(userId);
            string cardId = _tradingsRepository.GetCardIdByDealId(dealId);
            Console.WriteLine(cardId);
            _dbAccess.ExecuteTransaction((conn, transaction) =>
            {
                try
                {
                    using (var cmd = new NpgsqlCommand("UPDATE card_stacks SET user_id = @userId WHERE unique_id = @cardId", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@cardId", cardId);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("Error transferring card");
                }
            });
        }
    }
}
