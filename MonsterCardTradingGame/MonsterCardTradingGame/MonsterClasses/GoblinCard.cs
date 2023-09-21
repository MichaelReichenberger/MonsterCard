using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.BaseClasses;

namespace MonsterCardTradingGame.MonsterClasses
{
    internal class GoblinCard : MonsterCard
    {
        private const int GoblinDamage = 25;
        public GoblinCard(string name, string element) : base(name, element, GoblinDamage)
        {
        }
    }
}
