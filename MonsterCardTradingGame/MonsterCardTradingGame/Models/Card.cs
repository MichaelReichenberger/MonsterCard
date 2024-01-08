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

        private string _uniqueId;

        public string UniqueId
        {
            get { return _uniqueId; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _uniqueId = value;
                }
                else
                {
                    throw new ArgumentException("UniqueId kann nicht NULL oder EMPTY sein!");
                }
            }
        }
        
        public enum ElementType { Fire, Water, Normal }
        public GameManager.Element Element { get; set; }
        public double Damage { get; set; }
        public int CurrentWins { get; set; }
        public string Type { get; set; }
        public Card(string name, GameManager.Element element, double damage, string uniqueId)
        {   
            Name = name;
            if (name == "Spell")
            {
                Type = "Spell";
            }
            else
            {
                Type = "Monster";
            }
            Element = element;
            Damage = damage;
            UniqueId = uniqueId;
            CurrentWins = 0;
        }   
       
    }
}
