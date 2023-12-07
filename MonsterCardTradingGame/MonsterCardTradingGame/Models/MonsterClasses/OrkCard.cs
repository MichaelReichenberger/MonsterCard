using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Models.BaseClasses;

namespace MonsterCardTradingGame.Models.MonsterClasses
{
    internal class OrkCard : MonsterCard
    {
        private const int OrkDamage = 45;
        public OrkCard(string name, string element) : base(name, element, OrkDamage)
        {
        }

        public override int attack(Card OpponentCard)
        {
            if (OpponentCard is WizzardCard)
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
