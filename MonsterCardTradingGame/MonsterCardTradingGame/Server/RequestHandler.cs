﻿using MonsterCardTradingGame.Server.Routes;
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
            string requestMethod = string.Empty;
            string requestUrl = string.Empty;
            string body = string.Empty;
            int? contentLength = null;

            //Reading the Header
            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                Console.WriteLine(line);
                if (line.StartsWith("GET") || line.StartsWith("POST") || line.StartsWith("PUT") ||
                    line.StartsWith("DELETE"))
                {
                    requestMethod = line.Split(' ')[0];
                    requestUrl = ParseUrl(line);
                }

                if (line.StartsWith("Content-Length:"))
                {
                    contentLength = int.Parse(line.Split(' ')[1]);
                }
            }

            // Read the Body only when reading the Header is finished
            if (contentLength.HasValue)
            {
                char[] buffer = new char[contentLength.Value];
                reader.Read(buffer, 0, contentLength.Value);
                body = new string(buffer);
            }

            var router = new Router();
            var routeConfig = new RouteConfig(router);

            var response = router.HandleRequest(requestMethod, requestUrl, body);
            writer.WriteLine(response);
        }

    }
}
