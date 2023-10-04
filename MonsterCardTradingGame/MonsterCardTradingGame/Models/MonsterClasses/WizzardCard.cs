using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Models.MonsterClasses
{
    internal class WizzardCard : MonsterCard
    {

        private const int WizzardDamage = 35;
        public WizzardCard(string name, string element) : base(name, element, 35)
        {
        }

        public override int attack(Card OpponentCard)
        {
            return Damage;
        }
    }
}
