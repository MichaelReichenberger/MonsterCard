namespace MonsterCardTradingGame.Models
{
    internal class Player
    {
        public string Name { get; set; }

        public string PasswordHash { get; set; }

        public int PlayerId { get; set; }

        public int Coins { get; set; }

        public CardDeck CardDeck { get; set; }
        public CardStack CardStack { get; set; }
        public Player(string name, string passwordHash, CardDeck cardDeck)
        {
            Name = name;
            PasswordHash = passwordHash;
            Coins = 20;
            PlayerId++;
            CardDeck = cardDeck;
        }
    }
}
