using System;
using Npgsql;

namespace MonsterCardTradingGame.DataBase
{
    internal class DBAccess
    {
        private static string _connString;

        public DBAccess(string connString)
        {
            _connString = connString;
        }

        public void Connect()
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
            }
        }

        public void Close(NpgsqlConnection conn)
        {
                conn.Close();
        }

        public T ExecuteQuery<T>(Func<NpgsqlConnection, T> queryFunction)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                return queryFunction(conn);
            }
        }
    }
}