using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using MonsterCardTradingGame.BusinessLogic;
using MonsterCardTradingGame.Server.Routes;
using MonsterCardTradingGame.Server.Sessions;

namespace MonsterCardTradingGame.Server
{
    internal class RequestHandler
    {
        private TcpClient _client;

        public RequestHandler(TcpClient client)
        {
            _client = client;
        }

        public async void ProcessRequest()
        {
            Parser _parser = new Parser();
            using var writer = new StreamWriter(_client.GetStream()) { AutoFlush = true };
            using var reader = new StreamReader(_client.GetStream());

            string? line;
            string requestMethod = string.Empty;
            string requestUrl = string.Empty;
            string requestParameter = string.Empty;
            string body = string.Empty;
            int? contentLength = null;
            string authToken = null;

            // Read the Header
            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                if (line.StartsWith("GET") || line.StartsWith("POST") || line.StartsWith("PUT") ||
                    line.StartsWith("DELETE"))
                {
                    requestMethod = line.Split(' ')[0];
                    requestUrl = _parser.ParseUrl(line, 1);
                    requestParameter = _parser.ParseUrl(line, 2);
                }

                if (line.StartsWith("Content-Length:"))
                {
                    contentLength = int.Parse(line.Split(' ')[1]);
                }

                if (line.StartsWith("Authorization: Bearer"))
                {
                    authToken = line.Substring("Authorization: Bearer".Length).Trim();
                }
            }

            var sessionManager = SessionManager.Instance;
            var session = sessionManager.GetSessionByToken(authToken);
            if (session == null && requestUrl != "/sessions" && (requestUrl!="/users" && requestMethod != "POST"))
            {
                writer.WriteLine("HTTP/1.0 401 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                                 JsonSerializer.Serialize(new { Message = "Access token is missing or invalid" }));
                return;
            }

            // Read Body if given
            if (contentLength.HasValue)
            {
                char[] buffer = new char[contentLength.Value];
                reader.Read(buffer, 0, contentLength.Value);
                body = new string(buffer);
            }

            // Forward Request to Router
            var router = new Router();
            var routeConfig = new RouteConfig(router);
            var response = await router.HandleRequest(requestMethod, requestUrl, body, requestParameter, sessionManager.GetUserIDByToken(authToken));
            writer.WriteLine(response);
        }
    }
}
