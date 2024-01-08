using System;
using System.Data;
using Npgsql;

namespace MonsterCardTradingGame.DataBase
{
    public class DBAccess
    {
        private readonly string _connString;

        public DBAccess(string connString)
        {
            _connString = connString;
        }

        private NpgsqlConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(_connString);
            connection.Open();
            return connection;
        }

        public T ExecuteQuery<T>(Func<NpgsqlConnection, T> queryFunction)
        {
            using (var conn = CreateConnection())
            {
                return queryFunction(conn);
            } 
        }

        public void ExecuteTransaction(Action<NpgsqlConnection, NpgsqlTransaction> transactionAction)
        {
            using (var conn = CreateConnection())
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        transactionAction(conn, transaction);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }
            }
        }
    }
}