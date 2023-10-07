using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Server.Routes;

namespace MonsterCardTradingGame.Server
{
    internal class Router
    {
        private List<Route> routes = new List<Route>();

        public void AddRoute(string url, RouteAction action)
        {
            routes.Add(new Route(url, action));
        }
        public string HandleRequest(string url)
        {
            foreach (var route in routes)
            {
                if (route.Url == url)
                {
                    return route.Action();
                }
            }
            return NotFound();
        }
        private string NotFound()
        {
            return "HTTP/1.0 404 Not Found\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>404 Not Found</h1></body></html>";
        }
    }
}
