using System.Text.Json;
using MonsterCardTradingGame.BusinessLogic;
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
            _router.AddRoute("/", () =>
            {
                return new ResponseGenerator().GenerateResponse();
            });

            //Info Route
            _router.AddRoute("/about", () =>
            {
                return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>About Us</h1></body></html>";
            });

            //Home Route
            _router.AddRoute("/home", () =>
            {
                return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>Home</h1></body></html>";
            });

            //Start a Game
            _router.AddRoute("/startgame", () =>
            {
                Dictionary<string, Card> newDeck = new Dictionary<string, Card>();
                CardDeck cd = new CardDeck(newDeck);
                Player player1 = new Player ("Test", "ggg",cd);
                Player player2 = new Player("Test", "ggg", cd);
                _gameManager.GameLoop(player1, player2);
                return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>Game started</h1></body></html>";
            });

            //Draw Package
            _router.AddRoute("/drawPackage", () =>
            {
                _package = new Package("newPackage");
                _package.printPackage();
                JsonSerializerOptions newOptions = new JsonSerializerOptions{WriteIndented = true};
                string _packageString = JsonSerializer.Serialize(_package, newOptions);
                Console.WriteLine(_packageString); 
                return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>"+_packageString+"</h1></body></html>";
            });
        }
    }
}