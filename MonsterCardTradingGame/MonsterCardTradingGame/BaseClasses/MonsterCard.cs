using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.BaseClasses
{
    internal class MonsterCard : Card
    {
        protected MonsterCard(string name, string element, int damage) : base(name, element, damage)
        {
        }

        public void printDamage()
        {
            Console.WriteLine(Damage);
        }
    }
}
