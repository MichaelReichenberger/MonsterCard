namespace MonsterCardTradingGame.Models
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

        private string _element;
        public string Element
        {
            get { return _element;} set { _element = value; }
        }
        protected int Damage { get; set; }

        protected int test = 0;

        protected Card(string name, string element, int damage)
        {
            Name = name;
            Element = element;
            Damage = damage;
        }
        public abstract int attack(Card OpponentCard);

    }
}
