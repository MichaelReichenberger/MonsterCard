namespace MonsterCardTradingGame.Models
{
    public class Card
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
        
        public enum ElementType { Fire, Water, Normal }
        public GameManager.Element Element { get; set; }
        public double Damage { get; set; }
        public Card(string name, GameManager.Element element, double damage)
        {
            Name = name;
            Element = element;
            Damage = damage;
        }
       
    }
}
