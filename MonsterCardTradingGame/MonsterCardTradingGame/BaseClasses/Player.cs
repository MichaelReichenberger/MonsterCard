using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.BaseClasses
{
    internal class Player
    {
        public string Name { get; set; }
        private string PasswordHash { get; }
        private int PlayerId{ get;}
        private int Coins { get; set; }

        public Player(string name, string passwordHash)
        {
            Name = name;
            PasswordHash = passwordHash;
            Coins = 20;
        }

    }
}
