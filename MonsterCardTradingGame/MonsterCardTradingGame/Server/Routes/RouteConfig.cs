using System.Text.Json;
using MonsterCardTradingGame.BusinessLogic;
using MonsterCardTradingGame.DataBase.Repositories;
using MonsterCardTradingGame.Models.BaseClasses;

namespace MonsterCardTradingGame.Server.Routes
{
    internal class RouteConfig
    {
        private Router _router;
        private GameManager _gameManager;
        public Package _package;
        public RouteConfig(Router router)
        {
            _router = router;
            _gameManager = new GameManager();
            DefineRoutes();
        }

        private void DefineRoutes()
        {
            //Default Route
            _router.AddRoute("GET","/", (requestBody) =>
            {
                return new ResponseGenerator().GenerateResponse();
            });

            //Info Route
            _router.AddRoute("GET","/about", (requestBody) =>
            {
                return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>About Us</h1></body></html>";
            });

            //Home Route
            _router.AddRoute("GET","/home", (requestBody) =>
            {
                UserRepository newUserRep =
                    new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
                newUserRep.GetUserId("testUser");
                
                Console.WriteLine(newUserRep.GetUserId("hallo"));
                return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>Home</h1></body></html>";
            });

            //Start a Game
            _router.AddRoute("POST","/startgame", (requestBody) =>
            {
                Dictionary<string, Card> newDeck = new Dictionary<string, Card>();
                CardDeck cd = new CardDeck(newDeck);
                Player player1 = new Player ("Test", "ggg",cd);
                Player player2 = new Player("Test", "ggg", cd);
                _gameManager.GameLoop(player1, player2);
                return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>Game started</h1></body></html>";
            });


            //User and Session Routes
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/users", (requestBody) =>
            {
                return "User Registered";
            });

            _router.AddRoute("GET", "/users/username", (requestBody) =>
            {
                return "Test";
            });

            _router.AddRoute("PUT", "/users/username", (requestBody) =>
            {
                return "Test";
            });

            _router.AddRoute("POST", "/sessions", (requestBody) =>
            {
                return "Test";
            });
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



            //Draw Package
            _router.AddRoute("POST","/drawpackage",  (requestBody) =>
            {
                if (int.Parse(requestBody) < 20)
                {
                    return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>" + "Sorry not enough Coins" + "</h1></body></html>";
                }
                else
                {
                    _package = new Package("newPackage");
                    JsonSerializerOptions newOptions = new JsonSerializerOptions { WriteIndented = true };
                    string _packageString = JsonSerializer.Serialize(_package, newOptions);
                    Console.WriteLine(requestBody);
                    return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>" + requestBody + "</h1></body></html>";
                }
            });
        }
    }
}