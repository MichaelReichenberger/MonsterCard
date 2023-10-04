using System;
using System.Net.Http;
using System.Threading.Tasks;
using MonsterCardTradingGame.Server;

namespace MonsterCardTradingGame
{
   internal class Program
    {
        static async Task Main(string[] args)
        {
            //Initialising Server
            string[] prefixes = { "http://localhost:8080/" };
            SimpleRestServer newRestServer = new SimpleRestServer(prefixes);

            //Start Server on 2cnd Thread, so that he is not blocking the Main Thread
            Task.Run(() => newRestServer.Run());

            //Wait for Server to start
            await Task.Delay(1000);

            // Send HTTP Request
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://localhost:8080/hello");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Anfrage fehlgeschlagen: {e.Message}");
                }
            }

            //Stop Server
            newRestServer.Stop();
        }
    }
}