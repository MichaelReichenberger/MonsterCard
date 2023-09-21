using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.BaseClasses;

namespace MonsterCardTradingGame.MonsterClasses
{
    internal class WizzardCard : MonsterCard
    {

        private const int WizzardDamage = 35;
        public WizzardCard(string name, string element, int damage) : base(name, element, 35)
        {
        }
    }
}
