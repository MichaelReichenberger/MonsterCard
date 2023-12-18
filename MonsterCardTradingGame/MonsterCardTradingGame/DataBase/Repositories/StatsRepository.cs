using MonsterCardTradingGame.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterCardTradingGame.DataBase.Repositories
{
    internal class StatsRepository :IRepository
    {

        private DBAccess _dbAccess { get; set; }
        private Parser _parser;
        private UserRepository _userRepository;
        private TradingsRepository _tradingsRepository;
        public StatsRepository(string connectionString)
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

        public string GetStatsFromDB(int userId)
        {
            return _dbAccess.ExecuteQuery<string>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM stats WHERE user_id = @userId", conn))
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

        public string GetAllStatsOrderedByElo()
        {
            return _dbAccess.ExecuteQuery<string>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM stats ORDER BY elo DESC", conn))
                {
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
    }
}
