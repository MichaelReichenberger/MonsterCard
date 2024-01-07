namespace MonsterCardTradingGame.Models
{
    public class CardDeck
    {
        public List<Card> Deck { get; set; }

        public CardDeck(List<Card> deck)
        {
            Deck = deck;
        }
    }
}
