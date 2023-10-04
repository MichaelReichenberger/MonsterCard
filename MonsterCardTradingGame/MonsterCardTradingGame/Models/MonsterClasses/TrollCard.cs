using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Models.MonsterClasses
{
    internal class TrollCard : MonsterCard
    {

        private const int TrollDamage = 30;
        public TrollCard(string name, string element) : base(name, element, TrollDamage)
        {
        }

        public override int attack(Card OpponentCard)
        {
            return Damage;
        }
    }
}
