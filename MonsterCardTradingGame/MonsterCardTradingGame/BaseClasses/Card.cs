using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.BaseClasses
{
    internal  class Card
    {

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _name = value;
                }
                else
                {
                    throw new ArgumentException("Name kann nicht NULL oder EMPTY sein!");
                }
            }
        }

        protected string Element { get; set; }
        protected int Damage { get; set; }

        protected int test = 0;

        protected Card(string name, string element, int damage)
        {
            Name = name;
            Element = element;
            Damage = damage;
        }
    }
}
