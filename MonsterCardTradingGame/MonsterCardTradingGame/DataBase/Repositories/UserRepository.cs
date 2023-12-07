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
        private DBAccess _dbAccess { get; set; }
        public UserRepository(string connectionString)
        {
            _dbAccess = new DBAccess(connectionString);
        }

        internal void AddUser(int user_id,string username,)
        {
            int result = _dbAccess.ExecuteQuery<int>(conn =>
            {
                using (var cmd = new NpgsqlCommand("INSERT INTO users (username) VALUES (@username);", conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    return cmd.ExecuteNonQuery();
                }
            });
        }



        internal void DeleteUser(){}

        internal void UpdateUser() {}

        internal void CreateUserTable() {}

        internal void GetUserStack(){}

        internal void UpdateUserStack() {}

        internal int? GetUserId(string username)
        {
            return _dbAccess.ExecuteQuery(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT u.userid FROM users u WHERE u.username = @username;", conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    return (int?)cmd.ExecuteScalar();
                }
            });
        }

        internal Dictionary<string, object> GetUserData(string username)
        {
            return _dbAccess.ExecuteQuery(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT u.username, u.coins, u.level, c.card_id, c.name, c.element, c.damage FROM users u JOIN decks d ON u.deck_id = d.deck_id JOIN cards c ON d.card_id = c.card_id WHERE u.username = @username\r\n", conn))
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

    }
}
