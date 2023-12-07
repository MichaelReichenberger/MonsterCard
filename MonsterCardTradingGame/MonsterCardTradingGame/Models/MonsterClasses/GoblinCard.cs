using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Models.BaseClasses;

namespace MonsterCardTradingGame.Models.MonsterClasses
{
    internal class GoblinCard : MonsterCard
    {
        private const int GoblinDamage = 25;
        public GoblinCard(string name, string element) : base(name, element, GoblinDamage)
        {
        }

        public override int attack(Card OpponentCard)
        {
            if (OpponentCard is DragonCard)
            {
                return 0;
            }
            else
            {
                return Damage;
            }
        }
    }
}
