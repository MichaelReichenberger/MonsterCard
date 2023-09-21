using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.BaseClasses;

namespace MonsterCardTradingGame.MonsterClasses
{
    internal class KnightCard : MonsterCard
    {
        private const int KnightDamage = 40;
        public KnightCard(string name, string element) : base(name, element, KnightDamage)
        {
        }
    }
}
