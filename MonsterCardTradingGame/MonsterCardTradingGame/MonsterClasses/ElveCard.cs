using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.BaseClasses;

namespace MonsterCardTradingGame.MonsterClasses
{
    internal class ElveCard : MonsterCard
    {
        private const int ElveDamage = 20;
        public ElveCard(string name, string element) : base(name, element, ElveDamage)
        {
        }
    }
}
