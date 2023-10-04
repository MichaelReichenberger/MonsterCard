using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Models;

namespace MonsterCardTradingGame.BusinessLogic
{
    internal class GameManager
    {
        public void GameLoop(Player Player1, Player Player2)
        {
            var randomVar = new Random();
            while (Player1.CardDeck.Deck.Any() && !Player2.CardDeck.Deck.Any())
            {
                string Card1Choice = SelectCard(Player1, Player1.CardDeck);
                string Card2Choice = SelectCard(Player2, Player1.CardDeck);
                Card ChoosenCard1 = Player1.CardDeck.Deck[Card1Choice];
                Card ChoosenCard2 = Player2.CardDeck.Deck[Card2Choice];

                if (Fight(ChoosenCard1, ChoosenCard2) == "Card1Wins")
                {
                    Player2.CardDeck.Deck.Remove(ChoosenCard2.Name);
                }
                else if (Fight(ChoosenCard1, ChoosenCard2) == "Card2Wins")
                {
                    Player1.CardDeck.Deck.Remove(ChoosenCard1.Name);
                }
                else
                {
                    Player1.CardDeck.Deck.Remove(ChoosenCard1.Name);
                    Player2.CardDeck.Deck.Remove(ChoosenCard2.Name);
                }
            }
        }
        public string SelectCard(Player Player, CardDeck CardDeck)
        {
            Console.WriteLine(Player.Name + " Please select the Card you want to play: ");
            string cardChoice = Console.ReadLine();
            return cardChoice;
        }
        public string Fight(Card ChoosenCard1, Card ChoosenCard2)
        {
            if (ChoosenCard1.attack(ChoosenCard2) > ChoosenCard2.attack(ChoosenCard1))
            {
                return "Card1Wins";
            }
            else if (ChoosenCard1.attack(ChoosenCard2) < ChoosenCard2.attack(ChoosenCard1))
            {
                return "Card2Wins";
            }
            else
            {
                return "Both Dead";
            }
        }
    }
}
