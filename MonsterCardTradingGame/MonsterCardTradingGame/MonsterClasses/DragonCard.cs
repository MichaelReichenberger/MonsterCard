using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.BaseClasses;

namespace MonsterCardTradingGame.MonsterClasses
{
    internal class DragonCard : MonsterCard
    {
        private const int DragonDamage = 50;
        public DragonCard(string name, string element) : base(name, element, DragonDamage)
        {
        }
    }
}
