using System;
using System.Net;
using System.Text;
using System.Text.Json;
using MonsterCardTradingGame.BusinessLogic;
using MonsterCardTradingGame.DataBase.Repositories;

using MonsterCardTradingGame.Server.Sessions;
using Npgsql.Replication;
using MonsterCardTradingGame.Server.Sessions;
namespace MonsterCardTradingGame.Server.Routes
{
    public class RouteConfig
    {
        private Router _router;

        private GameManager _gameManager;
        private UserManager _userManager;
        private TransactionManager _transactionManager;
        private CardManager _cardManager;

        public RouteConfig(Router router)
        {
            _router = router;
            _gameManager = GameManager.Instance;
            _userManager = new UserManager();
            _transactionManager = new TransactionManager();
            _cardManager = new CardManager();
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
            _router.AddRoute("GET", "/users", (requestbody, requestParameter, userId) => _userManager.GetUserData(requestbody, requestParameter, userId));


            // CreateUser Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/users", (requestBody, requestParameter, r) => _userManager.RegisterUser(requestBody, requestParameter));


            //UserupdatePut Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("PUT", "/users", (requestBody, requestParameter, userId) => _userManager.UpdateUserData(requestBody,requestParameter,userId));
             

            //Login Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/sessions", (requestBody, requestParameter, r) => _userManager.HandleLogin(requestBody,requestParameter));


            //Create package Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/packages", (requestBody, requestParameter, userId) => _transactionManager.CreatePackage(requestBody,requestParameter, userId));


            //Aquire packages Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/transactions",(requestBody, requestParameter, userId)=> _transactionManager.AquirePackage(requestBody,requestParameter,userId));


            //Get user cards Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/cards", (requestBody, requestParameter, userId) => _cardManager.GetUserCards(requestBody,requestParameter,userId));


            //Create trading deal Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/tradings", (requestBody, requestParameter, userId) => _transactionManager.CreateTradingDeal(requestBody,requestParameter,userId));


            //Check trading deals Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/tradings", (requestBody, requestParameter, userId) => _transactionManager.GetActiveTradingDeals(userId));


            //Delete trading deal Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("DELETE","/tradings",(requestBody, requestParameter, userId)=> _transactionManager.DeleteTradingDeal(requestBody,requestParameter,userId));

            
            //Configure deck Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("PUT", "/deck", (requestBody, requestParameter, userId) =>_cardManager.ConfigureDeck(requestBody,requestParameter,userId));


            //Get user deck Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/deck", (requestBody, requestParameter, userId) => _cardManager.GetDeck(userId));


            //Read user_stats Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/stats",(requestBody, requestParameter, userId) => _userManager.GetUserStats(userId));


            //Get scoreboard Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("GET", "/scoreboard", (requestBody, requestParameter, userId) => _userManager.GetUserStats(userId));


            //Start battle Route
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            _router.AddRoute("POST", "/battle",  (requestBody, requestParameter, userId) =>
            {
                return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       JsonSerializer.Serialize(new { Message = "Battle started:", Result =  _gameManager.WaitForOtherPlayerAndStartBattle(userId) });
            });
        }
    }
}