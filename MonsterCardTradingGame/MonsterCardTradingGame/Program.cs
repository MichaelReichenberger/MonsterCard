// See https://aka.ms/new-console-template for more information

using MonsterCardTradingGame.BaseClasses;
using MonsterCardTradingGame.MonsterClasses;

namespace MonsterCardTradingGame
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            DragonCard newCard = new DragonCard("peter", "water");
            newCard.printDamage();
            Package newPackage = new Package("testPackage");
            newPackage.printPackage();
        }
    }
}
