using System.Text;
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
            _router.AddRoute("GET","/", (requestbody,requestParameter) =>
            {
                return new ResponseGenerator().GenerateResponse();
            });

            //Info Route
            _router.AddRoute("GET","/about", (requestbody, requestParameter) =>
            {
                return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>About Us</h1></body></html>";
            });

            //Home Route
            _router.AddRoute("GET","/home", (requestbody, requestParameter) =>
            {
                UserRepository newUserRep =
                    new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
                newUserRep.GetUserId("testUser");
                
                Console.WriteLine(newUserRep.GetUserId("hallo"));
                return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>Home</h1></body></html>";
            });

            //Start a Game
            _router.AddRoute("POST","/startgame", (requestbody, requestParameter) =>
            {
                Dictionary<string, Card> newDeck = new Dictionary<string, Card>();
                CardDeck cd = new CardDeck(newDeck);
                Player player1 = new Player ("Test", "ggg",cd);
                Player player2 = new Player("Test", "ggg", cd);
                _gameManager.GameLoop(player1, player2);
                return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>Game started</h1></body></html>";
            });


            //UserDataRoute
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/users", (requestbody, requestParameter) =>
            {
                UserRepository newUserRep = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
                var userData = newUserRep.GetUserData(requestParameter);

                StringBuilder userDataString = new StringBuilder();
                StringBuilder cardDataString = new StringBuilder();
                int cardCount = 1;

                foreach (var kvp in userData)
                {
                    if (kvp.Value is Dictionary<string, object> cardData)
                    {
                        cardDataString.AppendLine($"<b>Card-{cardCount}:</b><br>");
                        foreach (var cardKvp in cardData)
                        {
                            cardDataString.AppendLine($"{cardKvp.Key}: {cardKvp.Value}<br>");
                        }
                        cardCount++;
                    }
                    else
                    {
                        userDataString.AppendLine($"{kvp.Key}: {kvp.Value}<br>");
                        
                    }
                }
                userDataString.AppendLine("<b>CARDS</b><br>");

                string finalOutput = userDataString.ToString() + cardDataString.ToString();

                return $"HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>User Data for {requestParameter}</h1><p>{finalOutput}</p></body></html>";
            });


            //UserupdatePut Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("PUT", "/users/username", (requestbody, requestParameter) =>
            {
                return "Test";
            });
            _router.AddRoute("GET", "/sessions", (requestbody, requestParameter) =>
            {
                return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>" + "Sorry not enough Coins" + "</h1></body></html>";
            });
            //Draw Package
            _router.AddRoute("POST","/drawpackage",  (requestbody, requestParameter) =>
            {
                if (int.Parse(requestbody) < 20)
                {
                    return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>" + "Sorry not enough Coins" + "</h1></body></html>";
                }
                else
                {
                    _package = new Package("newPackage");
                    JsonSerializerOptions newOptions = new JsonSerializerOptions { WriteIndented = true };
                    string _packageString = JsonSerializer.Serialize(_package, newOptions);
                    Console.WriteLine(requestbody, requestParameter);
                    return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>" + requestbody + "</h1></body></html>";
                }
            });
        }
    }
}