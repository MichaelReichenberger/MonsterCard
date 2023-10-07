using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MonsterCardTradingGame.Server.Router;

namespace MonsterCardTradingGame.Server.Routes
{
    internal class Route
    {
        public string Url { get; }
        public RouteAction Action { get; }

        public Route(string url, RouteAction action)
        {
            Url = url;
            Action = action;
        }
    }
}
