using MonsterCardTradingGame.DataBase.Repositories;
using MonsterCardTradingGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.BusinessLogic
{
    internal class OR_Mapper
    {
        private DeckRepository _deckRepository;
        private CardsRepository _cardsRepository;
        private Parser _parser;
        public OR_Mapper()
        {
            _deckRepository = new DeckRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _cardsRepository = new CardsRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _parser = new Parser();
        }
        internal CardDeck ParseCardDeck(int userId)
        {
            List<string> deckString = _parser.ParseUniqueIds(_deckRepository.GetDeckUniqueIdsByUserId(userId).Split(",").ToList());
            if (deckString.Count >= 5)
            {
                deckString.RemoveAt(4);
            }
            deckString = deckString.Where(id => !String.IsNullOrEmpty(id)).ToList();
            List<Card> deckCards = new List<Card>();
            foreach (string cardId in deckString)
            {
                Console.WriteLine(cardId);
                try
                {
                    Console.WriteLine(_cardsRepository.GetCardModelFromDB(cardId).Name);
                    deckCards.Add(_cardsRepository.GetCardModelFromDB(cardId));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            CardDeck cardDeck = new CardDeck(deckCards);
            return cardDeck;
        }
    }
}
