using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
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
    int contentLength = 0;
    while ((line = reader.ReadLine()) != null)
    {
        Console.WriteLine(line);
        if (line == "")
        {
            break;
        }

        if (line.StartsWith("Content-Length"))
        {
            contentLength = int.Parse(line.Substring(16).Trim());
        }
        //read the content if existing
        if (contentLength > 0)
        {
            StringBuilder content = new();
            int totalBYtesRead = 0;
            var buffer = new char[1024];
            while (totalBYtesRead<contentLength)
            {
                var bytesRead = reader.Read(buffer, 0, contentLength);
                if (bytesRead == 0)
                {
                    break;
                }
                totalBYtesRead += bytesRead;
                content.Append(buffer, 0, bytesRead);
            }
            Console.WriteLine(content);
        }
    }
    Console.WriteLine();
//write HTTP-response
    writer.WriteLine("HTTP/1.0 200 OK");
    writer.WriteLine("Content-Type: text/html; charset=utf-8");
    writer.WriteLine();
    writer.WriteLine("<html><body><h1>Hello World</h1></body></html>");
}