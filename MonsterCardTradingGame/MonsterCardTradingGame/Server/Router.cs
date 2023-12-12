using System.Collections.Generic;
using System.Threading.Tasks;
using MonsterCardTradingGame.Server.Routes;
namespace MonsterCardTradingGame.Server
{
    internal class Router
    {
        private List<Route> routes = new List<Route>();

        public void AddRoute(string method, string url, RouteAction action)
        {
            routes.Add(new Route(method, url, action));
        }

        public void AddRoute(string method, string url, AsyncRouteAction action)
        {
            routes.Add(new Route(method, url, action));
        }

        public async Task<string> HandleRequest(string method, string url, string requestBody, string requestParameter)
        {
            foreach (var route in routes)
            {
                if (route.Method == method && route.Url == url)
                {
                    return await route.Execute(requestBody, requestParameter);
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