using MonsterCardTradingGame.BusinessLogic;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.DataBase.Repositories
{
    public class TradingsRepository : IRepository
    {


        private DBAccess _dbAccess { get; set; }
        private Parser _parser;
        private UserRepository _userRepository;
        public TradingsRepository(string connectionString)
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




        //Insert offer into tradings table
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void InsertCardsIntoTradings(int userId, string dealId, string cardToInsert, int minimumDamage)
        {
            _dbAccess.ExecuteTransaction((conn, transaction) =>
            {
                try
                {
                    using (var cmd = new NpgsqlCommand(
                               "INSERT INTO tradings (sender_id, deal_id, offered_card, minimum_damage) VALUES (@userId, @dealId, @cardToTrade, @minimumDamage)",
                               conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@dealId", dealId);
                        cmd.Parameters.AddWithValue("@cardToTrade", cardToInsert);
                        cmd.Parameters.AddWithValue("@minimumDamage", minimumDamage);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("Error while inserting cards into tradings");
                }
            });
        } 

        //Check out tradings table
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string CheckTradingDeals(int userId)
        {
            try
            {
                return _dbAccess.ExecuteQuery<string>(conn =>
                {
                    using (var cmd = new NpgsqlCommand("SELECT * FROM tradings WHERE sender_id != @userId", conn))
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
                throw new Exception("Error while checking trading deals");
            }
        }

        //Check if deal exists
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CheckIfDealExists(string dealId)
        {
            try
            {
                return _dbAccess.ExecuteQuery<bool>(conn =>
                {
                    using (var cmd = new NpgsqlCommand("SELECT EXISTS(SELECT 1 FROM tradings WHERE deal_id = @dealId)",
                               conn))
                    {
                        cmd.Parameters.AddWithValue("@dealId", dealId);
                        return (bool)cmd.ExecuteScalar();
                    }
                });
            }
            catch(Exception e)
            {
                throw new Exception("Error while checking if deal exists");
            }
        }


        //Remove deal from tradings table
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void DeleteById(string dealId)
        {
            _dbAccess.ExecuteTransaction((conn, transaction) =>
            {
                try
                {
                    using (var cmd = new NpgsqlCommand("DELETE FROM tradings WHERE deal_id = @dealId", conn))
                    {
                        cmd.Parameters.AddWithValue("@dealId", dealId);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("Error while removing deal");
                }
            });
        }


        //Get the cardId of the card that is offered in the deal
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetCardIdByDealId(string dealId)
        {
            try
            {
                return _dbAccess.ExecuteQuery<string>(conn =>
                {
                    using (var cmd = new NpgsqlCommand("SELECT offered_card FROM tradings WHERE deal_id = @dealId", conn))
                    {
                        cmd.Parameters.AddWithValue("@dealId", dealId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader[0].ToString();
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                throw new Exception("Error getting cardId by dealId");
            }
        }

        //Get the senderId of the deal
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int GetSenderIdByDealId(string dealId)
        {
            try
            {
                return _dbAccess.ExecuteQuery<int>(conn =>
                {
                    using (var cmd = new NpgsqlCommand("SELECT sender_id FROM tradings WHERE deal_id = @dealId", conn))
                    {
                        cmd.Parameters.AddWithValue("@dealId", dealId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Convert.ToInt32(reader[0]);
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                throw new Exception("Error getting senderId by dealId");
            }
        }
    }
}
