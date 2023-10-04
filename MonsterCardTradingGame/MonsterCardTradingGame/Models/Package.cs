using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Models.MonsterClasses;

namespace MonsterCardTradingGame.Models
{
    internal class Package
    {
        private const int PackageCosts = 20;
        public Dictionary<string, Card> Cards;


        //Package Konstruktor mit random Karten mit unterschiedlichen Wahrscheinlichkeiten
        public Package(string name)
        {
            var randomVar = new Random();
            Cards = new Dictionary<string, Card>();
            for (int i = 0; i < 5; i++)
            {
                string Cardname = Console.ReadLine();
                int randomInt = randomVar.Next(0, 1000);
                switch (randomInt)
                {
                    case var n when (n <= 50):
                        DragonCard newDragoncard = new DragonCard(Cardname, "water");
                        Cards["DragonCard"] = newDragoncard;
                        break;
                    case var n when (n <= 150):
                        KnightCard newKnightCard = new KnightCard(Cardname, "water");
                        Cards["KnightCard"] = newKnightCard;
                        break;
                    case var n when (n <= 350):
                        ElveCard newElveCard = new ElveCard(Cardname, "water");
                        Cards["ElveCard"] = newElveCard;
                        break;
                    case var n when (n <= 600):
                        GoblinCard newGoblinCard = new GoblinCard(Cardname, "water");
                        Cards["GoblinCard"] = newGoblinCard;
                        break;
                    case var n when (n <= 680):
                        OrkCard newOrkCard = new OrkCard(Cardname, "water");
                        Cards["OrkCard"] = newOrkCard;
                        break;
                    case var n when (n <= 850):
                        TrollCard newTrollCard = new TrollCard(Cardname, "water");
                        Cards["TrollCard"] = newTrollCard;
                        break;
                    case var n when (n <= 930):
                        KrakenCard newKrakenCard = new KrakenCard(Cardname, "water");
                        Cards["KrakenCard"] = newKrakenCard;
                        break;
                    case var n when (n <= 1000):
                        WizzardCard newWizzardCard = new WizzardCard(Cardname, "water");
                        Cards["WizzardCard"] = newWizzardCard;
                        break;
                    default:
                        break;
                }
            }
        }

        public void printPackage()
        {
            foreach (KeyValuePair<string, Card> Card in Cards)
            {
                Console.WriteLine(Card.Value.Name);
            }
        }
    }
}
