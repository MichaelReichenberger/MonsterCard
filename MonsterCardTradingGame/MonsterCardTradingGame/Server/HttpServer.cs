using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MonsterCardTradingGame.Server
{
    public delegate string RouteAction(string requestBody, string requestParameter, int userId);
    public delegate Task<string> AsyncRouteAction(string requestBody, string requestParameter, int userId);

    internal class HttpServer
    {
        private TcpListener _listener;

        public HttpServer(int port)
        {
            //initialise the listener
            _listener = new TcpListener(IPAddress.Loopback, port);
        }

        //start the server
        public void Start()
        {
            //start listening for requests
            _listener.Start();
            Console.WriteLine("Server started...");
            //loop forever accepting new requests
            while (true)
            {
                var clientSocket = _listener.AcceptTcpClient();
                //handle each request in a new thread
                ThreadPool.QueueUserWorkItem(state =>
                {
                    Console.WriteLine($"=================================={DateTime.Now}==================================");
                    Console.WriteLine($"Handling client on thread: {Thread.CurrentThread.ManagedThreadId}");
                    var handler = new RequestHandler(clientSocket);
                    handler.ProcessRequest();
                    Console.WriteLine("");
                });
            }
        }
    }
}