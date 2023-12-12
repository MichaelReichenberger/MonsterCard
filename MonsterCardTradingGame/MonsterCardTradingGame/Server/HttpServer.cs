using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MonsterCardTradingGame.Server
{
    public delegate string RouteAction(string requestBody, string requestParameter);
    public delegate Task<string> AsyncRouteAction(string requestBody, string requestParameter);

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
                ThreadPool.QueueUserWorkItem(state =>
                {
                    var handler = new RequestHandler(clientSocket);
                    handler.ProcessRequest();
                });
            }
        }
    }
}
