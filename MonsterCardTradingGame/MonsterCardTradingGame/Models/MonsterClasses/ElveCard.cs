using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Models.MonsterClasses
{
    internal class ElveCard : MonsterCard
    {
        private const int ElveDamage = 20;
        public ElveCard(string name, string element) : base(name, element, ElveDamage)
        {
        }

        public override int attack(Card OpponentCard)
        {
            return Damage;
        }
    }
}
