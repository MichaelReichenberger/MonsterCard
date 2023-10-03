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
                int randomInt = randomVar.Next(0, 1000);

                switch (randomInt)
                {
                    case var n when (n <= 50):
                        DragonCard newDragoncard = new DragonCard(Cardname, "water");
                        Cards[i] = newDragoncard;
                        break;
                    case var n when (n <= 150):
                        KnightCard newKnightCard = new KnightCard(Cardname, "water");
                        Cards[i] = newKnightCard;
                        break;
                    case var n when (n <= 350):
                        ElveCard newElveCard = new ElveCard(Cardname, "water");
                        Cards[i] = newElveCard;
                        break;
                    case var n when (n <= 600):
                        GoblinCard newGoblinCard = new GoblinCard(Cardname, "water");
                        Cards[i] = newGoblinCard;
                        break;
                    case var n when (n <= 680):
                        OrkCard newOrkCard = new OrkCard(Cardname, "water");
                        Cards[i] = newOrkCard;
                        break;
                    case var n when (n <= 850):
                        TrollCard newTrollCard = new TrollCard(Cardname, "water");
                        Cards[i] = newTrollCard;
                        break;
                    case var n when (n <= 930 ):
                        KrakenCard newKrakenCard = new KrakenCard(Cardname, "water");
                        Cards[i] = newKrakenCard;
                        break;
                    case var n when (n <= 1000):
                        WizzardCard newWizzardCard = new WizzardCard(Cardname, "water");
                        Cards[i] = newWizzardCard;
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
                Console.WriteLine(Cards[i].Name + Cards[i]);
            }
        }
    }
}
