﻿using System;
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
        private UserRepository _userRepository;
        private PackageRepository _packageRepository;
        private CardsRepository _cardsRepository;
        private TradingsRepository _tradingsRepository;
        public RouteConfig(Router router)
        {
            _router = router;
            _gameManager = GameManager.Instance;
            _userRepository = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _packageRepository = new PackageRepository("Host = localhost; Username = myuser; Password = mypassword; Database = mydb");
            _cardsRepository = new CardsRepository("Host = localhost; Username = myuser; Password = mypassword; Database = mydb");
            _tradingsRepository = new TradingsRepository("Host = localhost; Username = myuser; Password = mypassword; Database = mydb");
            DefineRoutes();
        }

        private void DefineRoutes()
        {
            //Default Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/",
                (requestbody, requestParameter, r) => { return new ResponseGenerator().GenerateResponse(); });

            //UserDataRoute
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/users", (requestbody, requestParameter, r) =>
            {
                var userData = _userRepository.GetUserData(requestParameter);
                var userAsJson= JsonSerializer.Serialize(userData);
                return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + userAsJson;
            });

            // CreateUser Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/users", (requestBody, requestParameter, r) =>
            {
                Console.WriteLine($"Handling client on thread: {Thread.CurrentThread.ManagedThreadId}");
                try
                {
                    var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                    if (userData == null || !userData.TryGetValue("Username", out var username) ||
                        !userData.TryGetValue("Password", out var password))
                    {
                        return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               JsonSerializer.Serialize(new { Message = "Missing Username or Password" });
                    }
                    string bio = userData.TryGetValue("Bio", out var bioValue) ? bioValue : "";
                    string image = userData.TryGetValue("Image", out var imageValue) ? imageValue : "";
                    _userRepository.AddUser(username, password, image, bio);
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
            _router.AddRoute("PUT", "/users", (requestBody, requestParameter, r) =>
            {
                try
                {
                    var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                    if (String.IsNullOrEmpty(requestParameter))
                    {
                        return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               JsonSerializer.Serialize(new { Message = "Missing username" });
                    }

                    string username = userData.TryGetValue("Name", out var usernameValue) ? usernameValue : "";
                    string bio = userData.TryGetValue("Bio", out var bioValue) ? bioValue : "";
                    string image = userData.TryGetValue("Image", out var imageValue) ? imageValue : "";
                    string password = userData.TryGetValue("Password", out var passwordValue) ? passwordValue : "";

                    _userRepository.UpdateUser(username, requestParameter, password, image, bio);
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
            _router.AddRoute("POST", "/sessions", (requestBody, requestParameter, r) =>
            {
                var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);
                Console.WriteLine(r);
                if (userData == null || !userData.TryGetValue("Username", out var username) ||
                    !userData.TryGetValue("Password", out var password))
                {
                    return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "Missing Username or Password" });
                }

                if (password != _userRepository.getPasswordByUsername(username))
                {
                    Console.WriteLine(password);
                    Console.WriteLine(_userRepository.getPasswordByUsername(username));
                }
                else
                {
                    var sessionManager = SessionManager.Instance;
                    var token = sessionManager.GenerateToken(username); // Token generieren
                    int userId = _userRepository.GetUserId(username).Value;
                    var sessionId = sessionManager.CreateSession(token, userId);
                    return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "User successfully logged in", Token = token, SessionId = sessionId });
                }

                return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       JsonSerializer.Serialize(new { Message = "Internal Error" });
            });

            //Create package Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/packages", (requestBody, requestParameter, r) =>
            {
                Parser newParser = new Parser();
                
                if (newParser.IsValidJson(requestBody))
                {
                    _packageRepository.DeserializeAndInsertPackageToDB(requestBody);
                    return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "Request processed successfully" });
                }
                else
                {
                    return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "Input is not correkt" });
                }
            });

            //Aquire packages Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/transactions",(requestBody, requestParameter, userId)=>
            {
                if (_packageRepository.GetPackageCount() > 0)
                {
                    if (userId > 0)
                    {
                        _packageRepository.TransferPackageToStack(userId);
                        _packageRepository.RemoveFirstPackage();
                        return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               JsonSerializer.Serialize(new { Message = "Request processed successfully" });
                    }
                    else
                    {
                        return "HTTP/1.0 412 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                            JsonSerializer.Serialize(new { Message = "Invalid user_id" });
                    }
                    
                }
                else
                {
                    return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "No packages available" });
                }
            });

            //Get user cards Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/cards", (requestBody, requestParameter, userId) =>
            {
                if (userId > 0)
                {
                    return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = _cardsRepository.GetCardsFromDB(userId) });
                }
                else
                {
                    return "HTTP/1.0 412 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "Invalid user_id" });
                }
            });


            //Creat trading deal Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/tradings", (requestBody, requestParameter, userId) =>
            {
                if (_tradingsRepository.CheckIfDealExists(requestParameter))
                {
                    if (_tradingsRepository.GetSenderIdByDealId(requestParameter) != userId)
                    {
                        _cardsRepository.TransferCard(requestParameter, userId);
                        return "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               JsonSerializer.Serialize(new { Message = "Get the Deal!" });
                    }
                    else
                    {
                        return "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               JsonSerializer.Serialize(new { Message = "It is not allowed to deal with your self!" });
                    }
                   
                }
                try
                {
                    var tradeData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(requestBody);

                    if (tradeData == null ||
                        !tradeData.TryGetValue("Id", out var idElement) ||
                        !tradeData.TryGetValue("CardToTrade", out var cardToTradeElement) ||
                        !tradeData.TryGetValue("MinimumDamage", out var minimumDamageElement))
                    {
                        return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               JsonSerializer.Serialize(new { Message = "Missing required fields" });
                    }

                    // Extrahieren Sie die Werte aus den JsonElement-Objekten
                    string id = idElement.GetString();
                    string cardToTrade = cardToTradeElement.GetString();
                    int minDamage = minimumDamageElement.GetInt32();

                    // Fügen Sie die Daten in die Datenbank ein
                    _tradingsRepository.InsertCardsIntoTradings(userId, id, cardToTrade, minDamage);

                    return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "Trading data successfully inserted" });
                }
                catch (Exception ex)
                {
                    return
                        "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                        JsonSerializer.Serialize(new { Message = $"An error occurred: {ex.Message}" });
                }
            });

            //Check trading deals Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/tradings", (requestBody, requestParameter, userId) =>
            {
                return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       JsonSerializer.Serialize(new {Trading = _tradingsRepository.CheckTradingDeals(userId) });
            });

            //Read user_stats Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/stats", (requestBody, requestParameter, r) =>
            {
                return
                    "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                    JsonSerializer.Serialize(new { Message = $"An error occurred: " });
            });
            //Start battle Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/battle", async (requestBody, requestParameter, r) =>
            {
                return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       JsonSerializer.Serialize(new { Message = "Battle started:", randomNumber = await _gameManager.WaitForOtherPlayerAndStartBattleAsync() });
            });
        }
    }
}