using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Models.BaseClasses;

namespace MonsterCardTradingGame.Models.MonsterClasses
{
    internal class DragonCard : MonsterCard
    {
        private const int DragonDamage = 50;
        public DragonCard(string name, string element) : base(name, element, DragonDamage)
        {
        }
        public override int attack(Card OpponentCard)
        {
            if (OpponentCard is ElveCard && OpponentCard.Element == "Fire")
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
