using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MonsterCardTradingGame.DataAccess.Repositories;
using MonsterCardTradingGame.DataBase.Repositories;
using MonsterCardTradingGame.Models;
using MonsterCardTradingGame.Server.Sessions;
using Newtonsoft.Json.Linq;
using Npgsql.Replication;

namespace MonsterCardTradingGame.BusinessLogic
{
    public class TransactionManager
    {
        private PackageRepository _packageRepository;
        private UserRepository _userRepository;
        private CardsRepository _cardsRepository;
        private TradingsRepository _tradingsRepository;

        public TransactionManager()
        {
            _packageRepository = new PackageRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _userRepository = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _cardsRepository = new CardsRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _tradingsRepository = new TradingsRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
        }


        //Create a new package
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string CreatePackage(string requestBody, string requestParameter, int userId)
        {
            try
            {
                Console.WriteLine(SessionManager.Instance.GetTokenByUserId(userId));
                
                //Check if user is admin
                if (_userRepository.GetRole(userId) == "admin")
                {
                    try
                    {
                        Parser newParser = new Parser();
                        if (newParser.IsValidJson(requestBody))
                        {
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            //Get new cards
                            var Newcards = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(requestBody, options);

                            //Get existing cards
                            List<Card> AllCards = _cardsRepository.GetAllCardsFromDB();
                            foreach (var card in Newcards)
                            {
                                //Check if card already exists
                                if (AllCards.FirstOrDefault(Card => Card.UniqueId == card["Id"].GetString()) !=
                                    null)
                                {
                                    return "HTTP/1.0 409 Conflict\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                                           "At least one card in the packages already exists";
                                }
                            }
                            _packageRepository.DeserializeAndInsertPackageToDB(requestBody);
                            return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                                   "Package and cards successfully created";
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    return "HTTP/1.0 500 Bad Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "Internal Error";
                }
                else
                {
                    return "HTTP/1.0 403 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "Provided user is not \"admin\"";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "HTTP/1.0 403 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       "Error creating package";
            }
        }


        //Aquire a package
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal string AquirePackage(string requestBody, string requestParameter, int userId)
        {
            try
            {
                //Check if there are packages available
                if (_packageRepository.GetPackageCount() > 0)
                {
                    if (userId > 0)
                    {
                        //Check if user has enough money
                        if (_userRepository.GetCoins(userId) < 5)
                        {
                            return "HTTP/1.0 403 \r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                                   "Not enough money for buying a card package";
                        }

                        //Aquire the package
                        _packageRepository.TransferPackageToStack(userId);
                        _packageRepository.RemoveFirstPackage();
                        _userRepository.SubstractCoins(userId, 5);
                        
                        return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +"A package has been successfully bought";
                    }

                    return "HTTP/1.0 412 ERR\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "Invalid user_id";

                }

                return "HTTP/1.0 404 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + "No card package available for buying";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return "HTTP/1.0 404 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                   "No packages available";
        }


        //Create or execute a trading deal
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string CreateOrExecuteTradingDeal(string requestBody, string requestParameter, int userId)
        {
            requestBody = requestBody.Trim('"');
            Console.WriteLine(requestParameter);

            //Create Deal
            if (requestParameter == "/")
            {
                try
                {
                    var tradeData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(requestBody);

                    //Check for required fields
                    if (tradeData == null ||
                        !tradeData.TryGetValue("Id", out var idElement) ||
                        !tradeData.TryGetValue("CardToTrade", out var cardToTradeElement) ||
                        !tradeData.TryGetValue("MinimumDamage", out var minimumDamageElement))
                    {
                        return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               "Missing required fields";
                    }

                    string id = idElement.GetString();

                    //Check if Deal already exists
                    if (_tradingsRepository.CheckIfDealExists(id))
                    {
                        return "HTTP/1.0 409 Conflict\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               "A deal with this deal ID already exists";
                    }

                    string cardToTrade = cardToTradeElement.GetString();
                    List<Card> userCards = _cardsRepository.GetCardsFromDB(userId);

                    //Check is user owns the card
                    if ((userCards.FirstOrDefault(Card => Card.UniqueId == cardToTrade ) == null))
                    {
                        return "HTTP/1.0 403 Forbidden\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               "The offered card is not owned by the user, or the requirements are not met (Type, MinimumDamage), or the offered card is locked in the deck, or the user tries to trade with self";
                    }
                    int minDamage = minimumDamageElement.GetInt32();

                    //Insert deal into DB
                    _tradingsRepository.InsertCardsIntoTradings(userId, id, cardToTrade, minDamage);
                    return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "Trading deal successfully created";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return
                        "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                        "Internal Error";
                }

                //Execute Deal
            }else if (_tradingsRepository.CheckIfDealExists(requestParameter))
            {
                //Check if user tries to trade with himself
                if (_tradingsRepository.GetSenderIdByDealId(requestParameter) != userId)
                {
                    Console.WriteLine(requestBody);
                    List<Card> userCards = _cardsRepository.GetCardsFromDB(userId);
                    //Check if user owns card
                    if ((userCards.FirstOrDefault(Card => Card.UniqueId == requestBody) == null))
                    {
                        return "HTTP/1.0 403 Forbidden\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               "The offered card is not owned by the user, or the requirements are not met (Type, MinimumDamage), or the offered card is locked in the deck, or the user tries to trade with self";
                    }
                    try
                    {
                        _cardsRepository.TransferReceivedCard(requestParameter, requestBody);
                        _cardsRepository.TransferCard(requestParameter, userId);
                        _tradingsRepository.DeleteById(requestParameter);
                        return "HTTP/1.0 201 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               "Trading deal successfully executed";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    return "HTTP/1.0 403 Forbidden\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "The offered card is not owned by the user, or the requirements are not met (Type, MinimumDamage), or the offered card is locked in the deck, or the user tries to trade with self";
                }
            }
            return "HTTP/1.0 404 Not found\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                   "The provided deal ID was not found";
        }


        //Get all active deals except the ones of the user
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal string GetActiveTradingDeals(int userId)
        {
            List<TradingDeal> tradingDeals = _tradingsRepository.CheckTradingDeals(userId);
            if (tradingDeals.Count == 0 || tradingDeals == null)
            {
                return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + "The request was fine, but there are no trading deals available";
            }
            return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + "There are trading deals available, the response contains these\r\n" +
                   JsonSerializer.Serialize(tradingDeals);
        }


        //Delete trading deal
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal string DeleteTradingDeal(string requestBody, string requestParameter, int userId)
        {
            //Check if deal exists and if the user is the sender
            if (_tradingsRepository.CheckIfDealExists(requestParameter) && _tradingsRepository.GetSenderIdByDealId(requestParameter) == userId)
            {
                _tradingsRepository.DeleteById(requestParameter);
                return
                    "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                    "Trading deal successfully deleted";
            }else if (!(_tradingsRepository.GetSenderIdByDealId(requestParameter) == userId))
            {
                return "HTTP/1.0 403 Forbidden\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       "The deal contains a card that is not owned by the user";
            }
            else if(!_tradingsRepository.CheckIfDealExists(requestParameter))
            { 
                return "HTTP/1.0 404 Not found\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                     "The provided deal ID was not found";
            }
            else
            {
                return "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       "An error occurred";
            }
        }
    }
    
}
    

