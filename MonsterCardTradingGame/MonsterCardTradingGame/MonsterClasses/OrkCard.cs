using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.BaseClasses;

namespace MonsterCardTradingGame.MonsterClasses
{
    internal class OrkCard : MonsterCard
    {
        private const int OrkDamage = 45;
        public OrkCard(string name, string element, int damage) : base(name, element, OrkDamage)
        {
        }
    }
}
