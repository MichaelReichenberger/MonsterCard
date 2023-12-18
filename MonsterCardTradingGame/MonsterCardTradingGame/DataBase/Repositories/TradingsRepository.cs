using MonsterCardTradingGame.BusinessLogic;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.DataBase.Repositories
{
    internal class TradingsRepository : IRepository
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
        public void InsertCardsIntoTradings(int userId, string dealId, string cardToInsert,int minimumDamage)
        {
            _dbAccess.ExecuteQuery<int>(conn =>
            {
                using (var cmd =
                       new NpgsqlCommand(
                           "INSERT INTO tradings (sender_id,deal_id, offered_card, minimum_damage) VALUES (@userId,@dealId, @cardToTrade, @minimumDamage)",
                           conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@dealId", dealId);
                    cmd.Parameters.AddWithValue("@cardToTrade", cardToInsert);
                    cmd.Parameters.AddWithValue("@minimumDamage", minimumDamage);
                    return cmd.ExecuteNonQuery();
                }
            });
        }

        //Check out tradings table
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string CheckTradingDeals(int userId)
        {
            return _dbAccess.ExecuteQuery<string>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM tradings WHERE sender_id = @userId", conn))
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

        //Check if deal exists
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CheckIfDealExists(string dealId)
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


        //Remove deal from tradings table
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RemoveDeal(string dealId)
        {
            _dbAccess.ExecuteQuery<int>(conn =>
            {
                using (var cmd = new NpgsqlCommand("DELETE FROM tradings WHERE deal_id = @dealId", conn))
                {
                    cmd.Parameters.AddWithValue("@dealId", dealId);
                    return cmd.ExecuteNonQuery();
                }
            });
        }


        //Get the cardId of the card that is offered in the deal
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetCardIdByDealId(string dealId)
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
                            // Nur der Wert der ersten Spalte (offered_card) wird zurückgegeben
                            return reader[0].ToString();
                        }
                        else
                        {
                            // Kein Eintrag gefunden, gebe einen leeren String oder Null zurück
                            return null;
                        }
                    }
                }
            });
        }

        //Get the senderId of the deal
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int GetSenderIdByDealId(string dealId)
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
                            // Nur der Wert der ersten Spalte (sender_id) wird zurückgegeben
                            return Convert.ToInt32(reader[0]);
                        }
                        else
                        {
                            // Kein Eintrag gefunden, gebe einen leeren String oder Null zurück
                            return -1;
                        }
                    }
                }
            });
        }
    }
}
