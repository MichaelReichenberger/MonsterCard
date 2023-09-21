using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.BaseClasses
{
    internal class SpellCard : Card
    {
        public SpellCard(string name, string element, int damage) : base(name, element, 20)
        {
        }
    }
}
