using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.BaseClasses;

namespace MonsterCardTradingGame.MonsterClasses
{
    internal class TrollCard : MonsterCard
    {

        private const int TrollDamage = 30;
        public TrollCard(string name, string element, int damage) : base(name, element, TrollDamage)
        {
        }
    }
}
