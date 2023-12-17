using MonsterCardTradingGame.BusinessLogic;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.DataBase.Repositories
{
    internal class DeckRepository : IRepository
    {

        private DBAccess _dbAccess { get; set; }
        private Parser _parser;
        private UserRepository _userRepository;
        public DeckRepository(string connectionString)
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

        public void InsertCardsIntoDeck(int userId, string reuestBody)
        {
            
        }
    }
}
