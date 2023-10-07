using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Models;
using MonsterCardTradingGame.Server;

namespace MonsterCardTradingGame
{
    internal class programm
    {
        static void Main(string[] args)
        {
            var server = new HttpServer(10001);
            server.Start();
        }
    }
}
