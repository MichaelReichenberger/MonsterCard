using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MonsterCardTradingGame.DataAccess.Repositories;
using MonsterCardTradingGame.DataBase.Repositories;
using MonsterCardTradingGame.Models;

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


        //Get cards from a specific user
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal string GetUserCards(string requestBody, string requestParameter, int userId)
        {
            if (userId > 0)
            {
                try
                { List<Card> cards = _cardsRepository.GetCardsFromDB(userId);
                    if(cards.Count == 0 || cards == null)
                        return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                               "The request was fine, but the user doesn't have any cards";
                    else
                    {
                        return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + "The user has cards, the response contains these\r\n " +
                               JsonSerializer.Serialize(cards);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            return "HTTP/1.0 401 ERR\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                   "Invalid user_id";
        }


        //Configure the deck of a specific user
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal string ConfigureDeck(string requestBody,string requestParameter, int userId)
        {
            try
            {

                var CardIds = JsonSerializer.Deserialize<List<string>>(requestBody);
                if (CardIds.Count == 4)
                {
                    foreach (var cardId in CardIds)
                    {
                        //Check if the user owns the card
                        if (!_cardsRepository.CheckIfUserOwnsCard(userId, cardId))
                            return
                                "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                                "\r\nAt least one of the provided cards does not belong to the user or is not available.";
                    }
                    _deckRepository.DeleteDeckByUserId(userId);
                    _deckRepository.InsertCardsIntoDeck(userId, CardIds);
                    return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "The deck has been successfully configured";
                }
                else
                {
                    return
                        "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                        "The provided deck did not include the required amount of cards";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return
                    "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                    "An error occurred";
            }
        }


        //Get the deck of a specific user
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal string GetDeck(int userId)
        {
            try
            {
                List<Card> deck = _deckRepository.GetDeckByUserId(userId);
                if (deck.Count == 0 || deck == null)
                {
                    return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "The request was fine, but the deck doesn't have any cards";
                }
                else if (deck.Count == 4)
                {
                    return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "The deck has cards, the response contains these\r\n" +
                           JsonSerializer.Serialize(deck);
                }
                else
                {
                    return "HTTP/1.0 500 Internal Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "Deck is corrupt";
                }
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
