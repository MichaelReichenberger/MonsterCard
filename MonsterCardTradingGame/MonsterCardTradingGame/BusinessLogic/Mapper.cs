using MonsterCardTradingGame.DataBase.Repositories;
using MonsterCardTradingGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.DataAccess.Repositories;

namespace MonsterCardTradingGame.BusinessLogic
{
    public class Mapper
    {
        private DeckRepository _deckRepository;
        private CardsRepository _cardsRepository;
        private Parser _parser;
        public Mapper()
        {
            _deckRepository = new DeckRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _cardsRepository = new CardsRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _parser = new Parser();
        }


        //Map the deck to deck object
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public CardDeck ParseCardDeck(int userId)
        {
            //Get the decks uniqueIds from the database
            List<string> deckString = _parser.ParseUniqueIds(_deckRepository.GetDeckUniqueIdsByUserId(userId).Split(",").ToList());
            if (deckString.Count >= 5)
            {
                deckString.RemoveAt(4);
            }
            deckString = deckString.Where(id => !String.IsNullOrEmpty(id)).ToList();
            List<Card> deckCards = new List<Card>();

            //Get the card model from the database for each card in the deck
            foreach (string cardId in deckString)
            {
                Console.WriteLine(cardId);
                try
                {
                    deckCards.Add(_cardsRepository.GetCardModelFromDB(cardId));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            foreach (Card card in deckCards)
            {
                Console.WriteLine(card.Name+"-"+card.Element+"-"+card.Damage);
            }
            CardDeck cardDeck = new CardDeck(deckCards);
            return cardDeck;
        }
    }
}
