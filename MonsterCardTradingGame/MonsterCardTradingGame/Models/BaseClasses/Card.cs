namespace MonsterCardTradingGame.Models.BaseClasses
{
    internal abstract  class Card
    {

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _name = value;
                }
                else
                {
                    throw new ArgumentException("Name kann nicht NULL oder EMPTY sein!");
                }
            }
        }

        public string Element { get; set; }
        public int Damage { get; set; }
        public Card(string name, string element, int damage)
        {
            Name = name;
            Element = element;
            Damage = damage;
        }
        public abstract int attack(Card OpponentCard);
    }
}
