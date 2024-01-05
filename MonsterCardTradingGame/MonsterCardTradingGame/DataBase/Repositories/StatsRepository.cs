using MonsterCardTradingGame.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterCardTradingGame.DataBase.Repositories
{
    public class StatsRepository :IRepository
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


        //Get single users stats from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetStatsFromDB(int userId)
        {
            return _dbAccess.ExecuteQuery<string>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM user_stats WHERE user_id = @userId", conn))
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


        //Get scoreboard from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetAllStatsOrderedByElo()
        {
            return _dbAccess.ExecuteQuery<string>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM user_stats ORDER BY elo DESC", conn))
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

        public int GetUserElo(int userId)
        {
            return _dbAccess.ExecuteQuery<int>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT elo FROM user_stats WHERE user_id = @userId", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return (int)cmd.ExecuteScalar();
                }
            });
        }

        public int GetUserWins(int userId)
        {
            return _dbAccess.ExecuteQuery<int>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT wins FROM user_stats WHERE user_id = @userId", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return (int)cmd.ExecuteScalar();
                }
            });
        }

        public int GetUserLosses(int userId)
        {
            return _dbAccess.ExecuteQuery<int>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT losses FROM user_stats WHERE user_id = @userId", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return (int)cmd.ExecuteScalar();
                }
            });
        }

        public string InsertWinStatsIntoDB(int userId, int elo)
        {
            try
            {
                return _dbAccess.ExecuteQuery<string>(conn =>
                {
                    using (var cmd = new NpgsqlCommand(
                               "INSERT INTO user_stats (user_id, elo, wins, losses) VALUES (@userId, @elo, @wins, @losses)",
                               conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@elo", GetUserElo(userId) + elo);
                        cmd.Parameters.AddWithValue("@wins", GetUserWins(userId) + 1);
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
                throw new Exception("Error while inserting stats into DB");
            }
        }
        public string UpdateWinStatsInDB(int userId, int elo)
        {
            try
            {
                return _dbAccess.ExecuteQuery<string>(conn =>
                {
                    using (var cmd = new NpgsqlCommand(
                               "UPDATE user_stats SET elo = @elo, wins = @wins WHERE user_id = @userId",
                               conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@elo", GetUserElo(userId) + elo);
                        cmd.Parameters.AddWithValue("@wins", GetUserWins(userId) + 1);
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
                throw new Exception("Error while updating stats in DB");
            }
        }


        public string UpdateLoseStatsInDB(int userId, int elo)
        {
            try
            {
                return _dbAccess.ExecuteQuery<string>(conn =>
                {
                    using (var cmd = new NpgsqlCommand(
                               "UPDATE user_stats SET elo = @elo, losses = @losses WHERE user_id = @userId",
                               conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@elo", GetUserElo(userId) - elo);
                        cmd.Parameters.AddWithValue("@losses", GetUserLosses(userId) + 1);
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
                throw new Exception("Error while updating stats in DB");
            }
        }
    }
}
