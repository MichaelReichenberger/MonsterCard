using System;
using System.Net;
using System.Text;
using System.Text.Json;
using MonsterCardTradingGame.BusinessLogic;
using MonsterCardTradingGame.DataBase.Repositories;
using MonsterCardTradingGame.Models.BaseClasses;
using MonsterCardTradingGame.Server.Sessions;
using Npgsql.Replication;
using MonsterCardTradingGame.Server.Sessions;
namespace MonsterCardTradingGame.Server.Routes
{
    internal class RouteConfig
    {
        private Router _router;
        private GameManager _gameManager;
        public RouteConfig(Router router)
        {
            _router = router;
            _gameManager = new GameManager();
            DefineRoutes();
        }

        private void DefineRoutes()
        {
            //Default Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/",
                (requestbody, requestParameter) => { return new ResponseGenerator().GenerateResponse(); });

            //UserDataRoute
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/users", (requestbody, requestParameter) =>
            {
                UserRepository newUserRep =
                    new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
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
                if (finalOutput != null)
                {
                    return $"HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>Data successfully retrieved <br>User Data for {requestParameter}</h1><p>{finalOutput}</p></body></html>";

                }
                else
                {
                    return $"HTTP/1.0 404 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>User not found</h1><p>{finalOutput}</p></body></html>";

                }
                
            });

            // CreateUser Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/users", (requestBody, requestParameter) =>
            {
                try
                {
                    var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                    if (userData == null || !userData.TryGetValue("Username", out var username) ||
                        !userData.TryGetValue("Password", out var password))
                    {
                        return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               JsonSerializer.Serialize(new { Message = "Missing Username or Password" });
                    }

                    UserRepository userRepository =
                        new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
                    userRepository.AddUser(username, password);

                    return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "User successfully created" });
                }
                catch (Exception ex)
                {
                    return
                        "HTTP/1.0 409 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                        JsonSerializer.Serialize(new { Message = $"An error occurred: {ex.Message}" });
                }
            });

            //UserupdatePut Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("PUT", "/users", (requestBody, requestParameter) =>
            {
                try
                {
                    var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                    if (userData == null || !userData.TryGetValue("Username", out var username) ||
                        !userData.TryGetValue("Password", out var password))
                    {
                        return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               JsonSerializer.Serialize(new { Message = "Missing Username or Password" });
                    }

                    UserRepository userRepository =
                        new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
                    userRepository.UpdateUser(username, requestParameter, password);

                    return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "User updated successfully" });
                }
                catch (Exception ex)
                {
                    return
                        "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                        JsonSerializer.Serialize(new { Message = $"An error occurred: {ex.Message}" });
                }
            });

            //Login Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/sessions", (requestBody, requestParameter) =>
            {
                var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                if (userData == null || !userData.TryGetValue("Username", out var username) ||
                    !userData.TryGetValue("Password", out var password))
                {
                    return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "Missing Username or Password" });
                }

                UserRepository userRepository =
                    new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");

                if (password != userRepository.getPasswordByUsername(username))
                {
                    Console.WriteLine(password);
                    Console.WriteLine(userRepository.getPasswordByUsername(username));
                }
                else
                {
                    var sessionManager = SessionManager.Instance;
                    var token = sessionManager.GenerateToken(); // Token generieren
                    var sessionId = sessionManager.CreateSession(token);
                    return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "User successfully logged in", Token = token, SessionId = sessionId });

                }

                return "HTTP/1.0 401 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>" +
                       "Internal Error" + "</h1></body></html>";
            });


            //Create package Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/packages", (requestBody, requestParameter) =>
            {
                Parser newParser = new Parser();
                PackageRepository packageRepository =
                    new PackageRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
                if (newParser.IsValidJson(requestBody))
                {
                    packageRepository.InsertJsonPackagesIntoDatabase(requestBody);

                }
                else
                {
                    return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "Input is not correkt" });
                }
                var packageList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(requestBody);
                if (packageList == null || packageList.Count == 0)
                {
                    return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "No package data received" });
                }
                else
                {
                    foreach (var package in packageList)
                    {
                        if (package.TryGetValue("Id", out var uniuque_card_id))
                        {
                            Console.WriteLine($"unique_card_id: {uniuque_card_id}");
                        }

                        if (package.TryGetValue("Name", out var cardName))
                        {
                            (string Element, string Name) Card = newParser.ParseCards(cardName.ToString());
                            Console.WriteLine($"Type: {Card.Name}");
                            Console.WriteLine($"Element: {Card.Element}");
                        }

                        if (package.TryGetValue("Damage", out var cardDamage))
                        {
                            Console.WriteLine($"Damage: {cardDamage}");
                        }
                    }
                }

                return "HTTP/1.0 200 OK\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>" +
                       "Request Processed Successfully" + "</h1></body></html>";
            });

            //Aquire packages Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/transactions",(requestBody, requestParameter)=>
            {
                PackageRepository newPackageRepository = new PackageRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");

                if (newPackageRepository.GetPackageCount() > 0)
                {
                    newPackageRepository.DeserializeAndInsertCards(newPackageRepository.GetFirstPackage().PackageContent);
                    newPackageRepository.RemoveFirstPackage();
                }
                Console.WriteLine(newPackageRepository.GetPackageCount());
                return "Test";
            });
        }
    }
}