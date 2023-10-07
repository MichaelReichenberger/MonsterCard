using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Models.Enums;
using MonsterCardTradingGame.Models.MonsterClasses;

namespace MonsterCardTradingGame.Models.BaseClasses
{
    internal class Package
    {
        private const int PackageCosts = 20;
        public Dictionary<string, Card> Cards { get; private set; }



        //Package Konstruktor mit random Karten mit unterschiedlichen Wahrscheinlichkeiten
        public Package(string name)
        {
            var randomCardType = new Random();
            var randomElement = new Random();
            Cards = new Dictionary<string, Card>();
            for (int i = 0; i < 5; i++)
            {
                int randomCardInt = randomCardType.Next(0, 1000);
                Elements RandomElements = new Elements();
                string Element = RandomElements.ChooseWithProbability().ToString();
                switch (randomCardInt)
                {
                    case var n when (n <= 50):
                        DragonCard newDragoncard = new DragonCard(Element+"Dragon", Element);
                        Cards["DragonCard"+i] = newDragoncard;
                        break;
                    case var n when (n <= 150):
                        KnightCard newKnightCard = new KnightCard(Element + "Knight", Element);
                        Cards["KnightCard"+i] = newKnightCard;
                        break;
                    case var n when (n <= 350):
                        ElveCard newElveCard = new ElveCard(Element + "Elve", Element);
                        Cards["ElveCard"+i] = newElveCard;
                        break;
                    case var n when (n <= 600):
                        GoblinCard newGoblinCard = new GoblinCard(Element + "Goblin", Element);
                        Cards["GoblinCard"+i] = newGoblinCard;
                        break;
                    case var n when (n <= 680):
                        OrkCard newOrkCard = new OrkCard(Element + "Ork", Element);
                        Cards["OrkCard"+i] = newOrkCard;
                        break;
                    case var n when (n <= 850):
                        TrollCard newTrollCard = new TrollCard(Element + "Troll", Element);
                        Cards["TrollCard"+i] = newTrollCard;
                        break;
                    case var n when (n <= 930):
                        KrakenCard newKrakenCard = new KrakenCard(Element + "Kraken", Element);
                        Cards["KrakenCard"+i] = newKrakenCard;
                        break;
                    case var n when (n <= 1000):
                        WizzardCard newWizzardCard = new WizzardCard(Element + "Wizzard", Element);
                        Cards["WizzardCard"+i] = newWizzardCard;
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
