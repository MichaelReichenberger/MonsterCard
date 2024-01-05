using Microsoft.VisualBasic;
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
    public enum CardType { Monster, Spell }
    public enum Element { Fire, Water, Normal }
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
        // Handle special cases
        if (playerOneCard.Name == "Goblin" && playerTwoCard.Name == "Dragon") return 1;
        if (playerTwoCard.Name == "Goblin" && playerOneCard.Name == "Dragon") return -1;
        if (playerOneCard.Name == "Wizard" && playerTwoCard.Name == "Ork") return -1;
        if (playerTwoCard.Name == "Wizard" && playerOneCard.Name == "Ork") return 1;
        if (playerOneCard.Name == "Knight" && playerTwoCard.Element == Element.Water && playerTwoCard.Name == "Spell") return 1;
        if (playerTwoCard.Name == "Knight" && playerOneCard.Element == Element.Water && playerOneCard.Name == "Spell") return -1;
        if (playerOneCard.Name == "Kraken" || playerTwoCard.Name == "Kraken") return 0; // Kraken is immune

        // Calculate damage modifiers for elemental advantage
        double damagePlayerOne = playerOneCard.Damage;
        double damagePlayerTwo = playerTwoCard.Damage;

        if (playerOneCard.Element == Element.Water && playerTwoCard.Element == Element.Fire) damagePlayerOne *= 2;
        if (playerOneCard.Element == Element.Fire && playerTwoCard.Element == Element.Water) damagePlayerOne /= 2;
        if (playerTwoCard.Element == Element.Water && playerOneCard.Element == Element.Fire) damagePlayerTwo *= 2;
        if (playerTwoCard.Element == Element.Fire && playerOneCard.Element == Element.Water) damagePlayerTwo /= 2;
        if (playerOneCard.Element == Element.Fire && playerTwoCard.Element == Element.Normal) damagePlayerOne *= 2;
        if (playerOneCard.Element == Element.Normal && playerTwoCard.Element == Element.Fire) damagePlayerOne /= 2;
        if (playerTwoCard.Element == Element.Fire && playerOneCard.Element == Element.Normal) damagePlayerTwo *= 2;
        if (playerTwoCard.Element == Element.Normal && playerOneCard.Element == Element.Fire) damagePlayerTwo /= 2;
        if (playerOneCard.Element == Element.Normal && playerTwoCard.Element == Element.Water) damagePlayerOne *= 2;
        if (playerOneCard.Element == Element.Water && playerTwoCard.Element == Element.Normal) damagePlayerOne /= 2;
        if (playerTwoCard.Element == Element.Normal && playerOneCard.Element == Element.Water) damagePlayerTwo *= 2;
        if (playerTwoCard.Element == Element.Water && playerOneCard.Element == Element.Normal) damagePlayerTwo /= 2;

        
        if (damagePlayerOne > damagePlayerTwo) return -1; 
        if (damagePlayerTwo > damagePlayerOne) return 1; 
        return 0; 
    }


    public List<string> PlayBattle(CardDeck player1Deck, CardDeck player2Deck)
    {
        List<string> battleResults = new List<string>();
        Random rand = new Random();

        while (player1Deck.Deck.Count > 0 && player2Deck.Deck.Count > 0)
        {
            int i = rand.Next(player1Deck.Deck.Count);
            int j = rand.Next(player2Deck.Deck.Count);

            if (CardFight(player1Deck.Deck[i], player2Deck.Deck[j]) == -1)
            {
                battleResults.Add($"Player 1 wins with {player1Deck.Deck[i].Name} against {player2Deck.Deck[j].Name}");
                player2Deck.Deck.Add(player1Deck.Deck[i]); 
                player1Deck.Deck.RemoveAt(i); 
            }
            else if (CardFight(player1Deck.Deck[i], player2Deck.Deck[j]) == 1)
            {
                battleResults.Add($"Player 2 wins with {player2Deck.Deck[j].Name} against {player1Deck.Deck[i].Name}");
                player1Deck.Deck.Add(player2Deck.Deck[j]); 
                player2Deck.Deck.RemoveAt(j); 
            }
            else
            {
                battleResults.Add($"Draw between {player1Deck.Deck[i].Name} and {player2Deck.Deck[j].Name}");
            }
        }
        return battleResults;
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
