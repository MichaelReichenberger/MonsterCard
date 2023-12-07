using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Server
{
    internal class ResponseGenerator
    {
        public string GenerateResponse()
        {
            StringBuilder response = new StringBuilder();
            response.AppendLine("HTTP/1.0 200 OK");
            response.AppendLine("Content-Type: text/html; charset=utf-8");
            response.AppendLine();
            response.AppendLine("<html><body><h1>Hello World</h1></body></html>");
            return response.ToString();
        }
    }
}
