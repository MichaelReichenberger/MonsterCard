using MonsterCardTradingGame.BusinessLogic;
using MonsterCardTradingGame.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameManager
{
    private int playerCount = 0;
    private int? sharedRandomNumber;
    private readonly object lockObject = new object();
    private static GameManager instance = null;
    private static readonly object instanceLockObject = new object();
    private static OR_Mapper _orMapper;
    private CardDeck _playerOneCards;
    private CardDeck _playerTwoCards;
    private List<string> _lastBattleResult; // Zum Speichern des letzten Battle-Ergebnisses

    private GameManager()
    {
        _orMapper = new OR_Mapper();
        _lastBattleResult = null; // Anfangs gibt es kein Battle-Ergebnis
    }

    public static GameManager Instance
    {
        get
        {
            lock (instanceLockObject)
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }
    }


    public int CardFight(Card playerOneCard, Card playerTwoCard)
    {
        
    }


    public List<string> PlayBattle(CardDeck player1Deck, CardDeck player2Deck)
    {
        List<string> battleResults = new List<string>();
        Console.WriteLine("Deck1Count: " + player1Deck.Deck.Count);
        Console.WriteLine("Deck2Count: " + player2Deck.Deck.Count);

        if (player1Deck.Deck.Count == 4 && player2Deck.Deck.Count == 4)
        {
            for (int i = 0; i <= player1Deck.Deck.Count-1; i++)
            {
                Console.WriteLine("===="+i);
                if (player1Deck.Deck[i].Damage > player2Deck.Deck[i].Damage)
                {
                    battleResults.Add("Player 1 wins");
                }
                else if (player2Deck.Deck[i].Damage > player1Deck.Deck[i].Damage)
                {
                    battleResults.Add("Player 2 wins");
                }
                else
                {
                    battleResults.Add("Draw");
                }
            }
            return battleResults;
        }
        else
        {
            return null;
        }
    }

    public async Task<string> WaitForOtherPlayerAndStartBattleAsync(int userId)
    {
        Console.WriteLine($"User {userId} is attempting to join the game");
        string res = "";

        lock (lockObject)
        {
            playerCount++;
            Console.WriteLine($"Current player count: {playerCount}");

            if (playerCount == 1)
            {
                Console.WriteLine("Player 1 joined, waiting for another player...");
                _playerOneCards = _orMapper.ParseCardDeck(userId);
            }
            else if (playerCount == 2)
            {
                Console.WriteLine("Player 2 joined, starting the battle...");
                _playerTwoCards = _orMapper.ParseCardDeck(userId);
            }
        }

        // Warten, bis beide Spieler beigetreten sind
        while (playerCount < 2)
        {
            await Task.Delay(100);
        }

       
        if (_playerOneCards == null || _playerTwoCards == null)
        {
            return "Error: One or both player decks were not properly initialized.";
        }

        
        if (_lastBattleResult != null && _lastBattleResult.Count > 0)
        {
            
            return String.Join("\n", _lastBattleResult);
        }

        // Kampf starten und Ergebnisse berechnen
        List<string> battleResult = PlayBattle(_playerOneCards, _playerTwoCards);
        if (battleResult == null)
        {
            return "Error: One or both player decks were not properly initialized.";
        }
        else
        {
            _lastBattleResult = battleResult;
            foreach (string result in battleResult)
            {
                res += result + "\n"; 
                Console.WriteLine(result);
            }
            return res;
        }
    }
}
