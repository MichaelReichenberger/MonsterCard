using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MonsterCardTradingGame.DataBase.Repositories;

namespace MonsterCardTradingGame.BusinessLogic
{
    internal class CardManager
    {
        private CardsRepository _cardsRepository;
        private DeckRepository _deckRepository;

        public CardManager()
        {
            _cardsRepository = new CardsRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _deckRepository = new DeckRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
        }

        internal string GetUserCards(string requestBody, string requestParameter, int userId)
        {
            if (userId > 0)
            {
                try
                {
                    return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = _cardsRepository.GetCardsFromDB(userId) });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            return "HTTP/1.0 401 ERR\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                   JsonSerializer.Serialize(new { Message = "Invalid user_id" });
        }

        internal string ConfigureDeck(string requestBody,string requestParameter, int userId)
        {
            try
            {

                var CardIds = JsonSerializer.Deserialize<List<string>>(requestBody);
                if (CardIds.Count == 4)
                {
                    foreach (var cardId in CardIds)
                    {
                        if (!_cardsRepository.CheckIfUserOwnsCard(userId, cardId))
                            return
                                "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                                JsonSerializer.Serialize(new { Message = "You dont own this card!" });
                    }
                    _deckRepository.InsertCardsIntoDeck(userId, CardIds);
                    return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "Deck successfully configured" });
                }

                return
                    "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                    JsonSerializer.Serialize(new { Message = "Error configuring deck" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return
                    "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                    JsonSerializer.Serialize(new { Message = $"An error occurred: {e.Message}" });
            }
        }

        internal string GetDeck(int userId)
        {
            try
            {

                return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       JsonSerializer.Serialize(new { Message = _deckRepository.GetDeckByUserId(userId) });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return
                    "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                    JsonSerializer.Serialize(new { Message = $"An error occurred: {e.Message}" });
            }
        }
    }
}
