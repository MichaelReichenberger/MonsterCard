using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Models.MonsterClasses
{
    internal class KnightCard : MonsterCard
    {
        private const int KnightDamage = 40;
        public KnightCard(string name, string element) : base(name, element, KnightDamage)
        {
        }

        public override int attack(Card OpponentCard)
        {
            return Damage;
        }
    }
}
