using MonsterCardTradingGame.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using MonsterCardTradingGame.Models;

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
        public UserStats GetStatsFromDB(int userId)
        {
            UserStats userStats = null;
            return _dbAccess.ExecuteQuery<UserStats>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM user_stats WHERE user_id = @userId", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        var result = new StringBuilder();
                        while (reader.Read())
                        {
                            string username = reader.GetString(reader.GetOrdinal("username"));
                            int gamesPlayed = reader.GetInt32(reader.GetOrdinal("games_played"));
                            int wins = reader.GetInt32(reader.GetOrdinal("wins"));
                            int losses = reader.GetInt32(reader.GetOrdinal("losses"));
                            double winLossRation = reader.GetDouble(reader.GetOrdinal("win_loose_ratio"));
                            int elo = reader.GetInt32(reader.GetOrdinal("elo"));

                            userStats = new UserStats(username, gamesPlayed, wins, losses, winLossRation, elo);
                        }
                    }
                    return userStats;
                }
            });
        }

       

        //Get scoreboard from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<UserStats> GetAllStatsOrderedByElo()
        {
            string admin = "admin";
            List<UserStats> scoreBoard = new List<UserStats>();
            return _dbAccess.ExecuteQuery<List<UserStats>>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM user_stats WHERE username != @admin ORDER BY elo", conn))
                {
                    cmd.Parameters.AddWithValue("@admin", admin);
                    using (var reader = cmd.ExecuteReader())
                    {
                        var cards = new List<Card>();
                        while (reader.Read())
                        {
                            string username = reader.GetString(reader.GetOrdinal("username"));
                            int gamesPlayed = reader.GetInt32(reader.GetOrdinal("games_played"));
                            int wins = reader.GetInt32(reader.GetOrdinal("wins"));
                            int losses = reader.GetInt32(reader.GetOrdinal("losses"));
                            double winLossRation = reader.GetDouble(reader.GetOrdinal("win_loose_ratio"));
                            int elo = reader.GetInt32(reader.GetOrdinal("elo"));
                            
                            UserStats userStats = new UserStats(username, gamesPlayed, wins, losses, winLossRation, elo);
                            scoreBoard.Add(userStats);
                        }
                        return scoreBoard;
                    }
                }
            });
        }

        
        //Update stats (in case of win of a game) iun DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string UpdateWinStatsInDB(int userId)
        {
            try
            {
                return _dbAccess.ExecuteQuery<string>(conn =>
                {
                    using (var cmd = new NpgsqlCommand(
                               "UPDATE user_stats SET elo = elo + 3, wins = wins + 1, games_played = games_played +1, win_loose_ratio = CASE WHEN losses = 0 THEN wins + 1 ELSE (wins + 1) / NULLIF(losses, 0) END WHERE user_id = @userId;",
                               conn))
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
                Console.WriteLine(e.Message);
                throw new Exception("Error while updating stats in DB");
            }
        }


        //Update stats (in case of loss of a game) in DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string UpdateLoseStatsInDB(int userId)
        {
            try
            {
                return _dbAccess.ExecuteQuery<string>(conn =>
                {
                    using (var cmd = new NpgsqlCommand(
                               "UPDATE user_stats SET elo = elo - 5, losses = losses + 1, win_loose_ratio = wins/(losses+1), games_played = games_played +1 WHERE user_id = @userId",
                               conn))
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
                throw new Exception("Error while updating stats in DB");
            }
        }
    }
}
