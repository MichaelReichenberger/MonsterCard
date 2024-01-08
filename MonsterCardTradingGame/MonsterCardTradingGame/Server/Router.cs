using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using MonsterCardTradingGame.Server.Routes;
namespace MonsterCardTradingGame.Server
{
    public class Router
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

        public async Task<string> HandleRequest(string method, string url, string requestBody, string requestParameter, int userId)
        {
            foreach (var route in routes)
            {
                if (route.Method == method && route.Url == url)
                {
                    return await route.Execute(requestBody, requestParameter, userId);
                }
            }
            return NotFound();
        }

        private string NotFound()
        {
            return "HTTP/1.0 404 ERR\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                   JsonSerializer.Serialize(new { Message = "Not found" });
        }
    }
}