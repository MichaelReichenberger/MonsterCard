using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MonsterCardTradingGame.DataBase.Repositories;
using Npgsql.Replication;

namespace MonsterCardTradingGame.BusinessLogic
{
    internal class TransactionManager
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

        internal string CreatePackage(string requestBody, string requestParameter)
        {
            try
            {
                Parser newParser = new Parser();
                if (newParser.IsValidJson(requestBody))
                {
                    _packageRepository.DeserializeAndInsertPackageToDB(requestBody);
                    return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "Package and cards successfully created";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                   "Error inserting package. CardId may be a duplicate";
        }

        internal string AquirePackage(string requestBody, string requestParameter, int userId)
        {
            try
            {
                if (_packageRepository.GetPackageCount() > 0)
                {
                    if (userId > 0)
                    {
                        if (_userRepository.GetCoins(userId) < 20)
                        {
                            return "HTTP/1.0 403 \r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                                   "Not enough money for buying a card package";
                        }

                        _packageRepository.TransferPackageToStack(userId);
                        _packageRepository.RemoveFirstPackage();
                        _userRepository.SubstractCoins(userId, 20);
                        
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

        internal string CreateTradingDeal(string requestBody, string requestParameter, int userId)
        {
            if (_tradingsRepository.CheckIfDealExists(requestParameter))
            {
                if (_tradingsRepository.GetSenderIdByDealId(requestParameter) != userId)
                {
                    try
                    {
                        _cardsRepository.TransferReceivedCard(requestParameter, requestBody);
                        _cardsRepository.TransferCard(requestParameter, userId);
                        _tradingsRepository.DeleteById(requestParameter);
                        return "HTTP/1.0 201 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               "Deal done!";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    return "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "It is not allowed to deal with your self!";
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
                           "Missing required fields";
                }

                string id = idElement.GetString();
                string cardToTrade = cardToTradeElement.GetString();
                int minDamage = minimumDamageElement.GetInt32();

                _tradingsRepository.InsertCardsIntoTradings(userId, id, cardToTrade, minDamage);

                return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       "Trading data successfully inserted";
            }
            catch (Exception ex)
            {
                return
                    "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                    JsonSerializer.Serialize(new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        internal string GetActiveTradingDeals(int userId)
        {
            return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                   JsonSerializer.Serialize(new { Trading = _tradingsRepository.CheckTradingDeals(userId) });
        }

        internal string DeleteTradingDeal(string requestBody, string requestParameter, int userId)
        {
            if (_tradingsRepository.CheckIfDealExists(requestParameter))
            {
                _tradingsRepository.DeleteById(requestParameter);
                return
                    "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                    "Deal deleted";
            }
            return
                "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                "Error deleting deal";
        }
    }
    
}
    

