namespace MonsterCardTradingGame.Models
{
    internal class User
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public string Bio { get; set; }
        public int Coins { get; set; }
        public int Level { get; set; }
        
        public User()
        {
            
        }
    }
}
