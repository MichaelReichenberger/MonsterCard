using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.MonsterClasses;

namespace MonsterCardTradingGame.BaseClasses
{
    internal class Package
    {
        private const int PackageCosts = 20;
        public Card[] Cards;


        //Package Konstruktor mit random Karten mit unterschiedlichen Wahrscheinlichkeiten
        public Package(string name)
        {
            Cards = new Card[5]; 
            var randomVar = new Random(); 

            for (int i = 0; i < 5; i++)
            {
                string Cardname = Console.ReadLine();
                int randomInt = randomVar.Next(0, 100);

                switch (randomInt)
                {
                    case var n when (n <= 10):
                        DragonCard newDragoncard = new DragonCard(Cardname, "water");
                        Cards[i] = newDragoncard;
                        break;
                    case var n when (n <= 50):
                        KnightCard newKnightCard = new KnightCard(Cardname, "water");
                        Cards[i] = newKnightCard;
                        break;
                    case var n when (n <= 100):
                        ElveCard newElveCard = new ElveCard(Cardname, "water");
                        Cards[i] = newElveCard;
                        break;
                    default:
                        break;
                }
            }
        }


        public void printPackage()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(Cards[i].Name);
            }
        }
    }
}
