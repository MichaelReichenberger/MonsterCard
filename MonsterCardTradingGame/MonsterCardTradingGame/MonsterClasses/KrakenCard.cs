using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.BaseClasses;

namespace MonsterCardTradingGame.MonsterClasses
{
    internal class KrakenCard : MonsterCard
    {

        private const int KrakenDamage = 35;
        public KrakenCard(string name, string element) : base(name, element, KrakenDamage)
        {
        }
    }
}
