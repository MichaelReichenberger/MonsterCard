namespace MonsterCardTradingGame.Models
{
    internal abstract class MonsterCard : Card
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
