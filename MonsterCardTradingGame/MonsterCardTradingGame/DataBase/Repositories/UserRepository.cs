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

        internal void AddUser(){}

        internal void DeleteUser(){}

        internal void UpdateUser() {}

        internal void CreateUserTable() {}

        internal void GetUserStack(){}

        internal void UpdateUserStack() {}

        internal int? GetUserId(string username)
        {
            return _dbAccess.ExecuteQuery(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT username FROM users WHERE username = @username", conn)) // Hier wird die Verbindung zugewiesen.
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    return (int?)cmd.ExecuteScalar();
                }
            });
        }
    }
}
