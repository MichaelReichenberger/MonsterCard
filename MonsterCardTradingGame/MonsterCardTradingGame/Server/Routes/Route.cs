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
        public string Method { get; }
        public string Url { get; }
        public RouteAction Action { get; }
        public string Execute(string requestBody)
        {
            return Action(requestBody);
        }
        public Route(string method, string url, RouteAction action)
        {
            Method = method;
            Url = url;
            Action = action;
        }
    }
}

