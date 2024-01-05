﻿using MonsterCardTradingGame.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using MonsterCardTradingGame.Models;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace MonsterCardTradingGame.DataBase.Repositories
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

        //Get user cards from DB
        public string GetCardsFromDB(int userId)
        {
            return _dbAccess.ExecuteQuery<string>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM card_stacks WHERE user_id = @userId", conn))
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


        public Card GetCardModelFromDB(string uniqueId)
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
                            string Name = reader.GetString(reader.GetOrdinal("name"));
                            string Element = reader.GetString(reader.GetOrdinal("element"));
                            double Damage = reader.GetDouble(reader.GetOrdinal("damage"));
                            Card card = new Card(Name, Element, Damage);
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


        public string GetDeckInfosFromDB(int userId)
        {
            return _dbAccess.ExecuteQuery<string>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM card_stacks WHERE unique_id IN (SELECT unique_id FROM decks WHERE user_id = @userId)", conn))
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
