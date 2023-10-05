using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using MonsterCardTradingGame.Server;

var httpServer = new TcpListener(IPAddress.Loopback, 10001);
httpServer.Start();
while (true)
{

    var clientSocket = httpServer.AcceptTcpClient();
    using var writer = new StreamWriter(clientSocket.GetStream()) { AutoFlush = true };
    using var reader = new StreamReader(clientSocket.GetStream());

//read the request
    string? line;
    while ((line = reader.ReadLine()) != null)
    {
        Console.WriteLine(line);
    }

//write HTTP-response
    writer.WriteLine("HTTP/1.0 200 OK");
    writer.WriteLine("Contend-Type: text/html; charset=utf-8");
    writer.WriteLine();
    writer.WriteLine("<html> <body> <h1> Hello World</h1></body> </html>");

}