namespace MonsterCardTradingGame.Models.BaseClasses
{
    internal class SpellCard : Card
    {
        public SpellCard(string name, string element, int damage) : base(name, element, 20)
        {
        }
        public override int attack(Card OpponentCard)
        {
            throw new NotImplementedException();
        }
    }
}
