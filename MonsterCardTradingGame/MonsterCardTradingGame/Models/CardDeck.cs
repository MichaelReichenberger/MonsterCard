namespace MonsterCardTradingGame.Models
{
    internal class CardDeck
    {
        public Dictionary<string, Card> Deck { get; set; }

        public CardDeck(Dictionary<string, Card> deck)
        {
            Deck = deck;
        }
    }
}
