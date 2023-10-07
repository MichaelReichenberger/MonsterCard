using MonsterCardTradingGame.Models.BaseClasses;

namespace MonsterCardTradingGame.Models.MonsterClasses
{
    internal class KrakenCard : MonsterCard
    {

        private const int KrakenDamage = 35;
        public KrakenCard(string name, string element) : base(name, element, KrakenDamage)
        {
        }

        public override int attack(Card OpponentCard)
        {
            return Damage;

        }
    }
}
