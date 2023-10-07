using MonsterCardTradingGame.Server.Routes;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace MonsterCardTradingGame.Server
{
    internal class RequestHandler
    {
        private TcpClient _client;

        public RequestHandler(TcpClient client)
        {
            _client = client;
        }
        private string ParseUrl(string requestLine)
        {
            var parts = requestLine.Split(' ');
            if (parts.Length > 1)
            {
                return parts[1];
            }
            return "/";
        }
        public void ProcessRequest()
        {
            using var writer = new StreamWriter(_client.GetStream()) { AutoFlush = true };
            using var reader = new StreamReader(_client.GetStream());
            string? line;
            string requestUrl = string.Empty;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
                if (line.StartsWith("GET") || line.StartsWith("POST") || line.StartsWith("PUT") || line.StartsWith("DELETE"))
                {
                    requestUrl = ParseUrl(line);
                }

                if (line == "")
                {
                    break;
                }
            }

            var router = new Router();
            var routeConfig = new RouteConfig(router);

            var response = router.HandleRequest(requestUrl);
            writer.WriteLine(response);

        }
    }
}