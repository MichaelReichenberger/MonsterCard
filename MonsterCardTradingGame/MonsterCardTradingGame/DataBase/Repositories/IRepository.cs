using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.DataBase.Repositories
{
    internal interface IRepository
    {
        public int GetFirstId();
        public int GetNextId();
    }
}
