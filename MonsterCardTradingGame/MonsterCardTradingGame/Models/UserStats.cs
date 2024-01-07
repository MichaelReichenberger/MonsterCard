using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Models
{
    public class UserStats
    { 
        public string Username {get; set;}
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public double WinLossRatio { get; set; }
        public int Elo { get; set; }

        public UserStats(string username, int gamesPlayed, int wins,int losses, double winLossRatio,int elo)
        {
            Username = username;
            GamesPlayed = gamesPlayed;
            Wins = wins;
            Losses = losses;
            WinLossRatio = winLossRatio;
            Elo = elo;
        }
    }
}
