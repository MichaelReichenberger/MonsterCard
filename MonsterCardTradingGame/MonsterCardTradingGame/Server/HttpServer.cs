using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Server.Sessions;

namespace MonsterCardTradingGame.Server
{
    public delegate string RouteAction(string requestbody, string requestParameter);

    internal class HttpServer
    {
        private TcpListener _listener;
        
        public HttpServer(int port)
        {
            _listener = new TcpListener(IPAddress.Loopback, port);
        }
        public void Start()
        {
            _listener.Start();
            Console.WriteLine("Server started...");
            while (true)
            {
                var clientSocket = _listener.AcceptTcpClient();
                var handler = new RequestHandler(clientSocket);
                handler.ProcessRequest();
            }
        }
    }
}
