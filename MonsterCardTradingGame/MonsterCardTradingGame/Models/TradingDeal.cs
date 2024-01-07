using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Models
{
    public class TradingDeal
    {
        public string DealId { get; set; }
        public string CardToTrade { get; set; }
        public string Type { get; set; }
        public int MinimumDamage { get; set; }

        public TradingDeal(string dealId, string cardToTrade, string type, int minimumDamage)
        {
            DealId = dealId;
            CardToTrade = cardToTrade;
            Type = type;
            MinimumDamage = minimumDamage;
        }
    }
}
