using Microsoft.VisualBasic;
using MonsterCardTradingGame.BusinessLogic;
using MonsterCardTradingGame.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonsterCardTradingGame.DataBase.Repositories;

public class GameManager
{
    private int playerCount = 0;
    private readonly object lockObject = new object();
    private static GameManager instance = null;
    private static readonly object instanceLockObject = new object();

    private static OR_Mapper _orMapper;
    private CardDeck _playerOneCards;
    private CardDeck _playerTwoCards;

    private List<string> _lastBattleResult; 
    private bool _battleStarted; 
    private string _username1;
    private string _username2;
    private int _userId1;
    private int _userId2;
    public enum CardType { Monster, Spell }
    public enum Element { Fire, Water, Normal }

    private UserRepository _userRepository;
    private StatsRepository _statsRepository;

    
    private GameManager()
    {
        _orMapper = new OR_Mapper();
        _lastBattleResult = null;  
        _userRepository = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
        _statsRepository = new StatsRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
        _username1 = default;
        _username2 = default;
        _userId1 = default;
        _userId2 = default;
        _battleStarted = false;

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

    public List<string> PlayBattle(CardDeck player1Deck, CardDeck player2Deck, string username1, string username2, int userId1, int userId2)
    {
        List<string> battleResults = new List<string>();
        Random rand = new Random();
        string winner;
        while (player1Deck.Deck.Count > 0 && player2Deck.Deck.Count > 0)
        {
            int i = rand.Next(player1Deck.Deck.Count);
            int j = rand.Next(player2Deck.Deck.Count);

            if (player1Deck.Deck.Count > i && player2Deck.Deck.Count > j)
            {
                Card player1Card = player1Deck.Deck[i];
                Card player2Card = player2Deck.Deck[j];
                int fightResult = CardFight(player1Card, player2Card);

                if (fightResult == -1)
                {
                    battleResults.Add($"{username1} wins with {player1Card.Name} against {player2Card.Name}");
                    player2Deck.Deck.Add(player1Card);
                    player1Deck.Deck.Remove(player1Card);
                }
                else if (fightResult == 1)
                {
                    battleResults.Add($"{username2} wins with {player2Card.Name} against {player1Card.Name}");
                    player1Deck.Deck.Add(player2Card);
                    player2Deck.Deck.Remove(player2Card);
                }
                else
                {
                    battleResults.Add($"Draw between {player1Card.Name} and {player2Card.Name}");
                }
            }
        }
        if (player1Deck.Deck.Count == 0)
        {
            try
            {
                _statsRepository.UpdateWinStatsInDB(userId2);
                _statsRepository.UpdateLoseStatsInDB(userId1);
                battleResults.Add($"{username2} wins the game!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        else if (player2Deck.Deck.Count == 0)
        {
            try
            {
                _statsRepository.UpdateWinStatsInDB(userId1);
                _statsRepository.UpdateLoseStatsInDB(userId2);
                battleResults.Add($"{username1} wins the game!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        else
        {
            battleResults.Add($"Current score: {username1} {player1Deck.Deck.Count} - {player2Deck.Deck.Count} {username2}");
        }
        _lastBattleResult = battleResults;
        return battleResults;
    }

    public string WaitForOtherPlayerAndStartBattle(int userId)
    {
        Console.WriteLine($"User with ID {userId} is attempting to join the game");
        string res = "";
        
        lock (lockObject)
        {
            playerCount++;
            Console.WriteLine($"Current player count: {playerCount}");

            if (playerCount == 1)
            {   
                Console.WriteLine("User 1 joined, waiting for another player...");
                _playerOneCards = _orMapper.ParseCardDeck(userId);
                _username1 = _userRepository.GetUsername(userId);
                _userId1 = userId;
                Console.WriteLine(_username1);
                Monitor.Wait(lockObject);
            }
            else if (playerCount == 2)
            {   
                Console.WriteLine("User 2 joined, starting the battle...");
                _playerTwoCards = _orMapper.ParseCardDeck(userId);
                _username2 = _userRepository.GetUsername(userId);
                _userId2 = userId;
                Console.WriteLine(_username2);
                Monitor.Pulse(lockObject);
            }

            
            if (_battleStarted == false && _playerOneCards != null && _playerTwoCards != null 
                && !String.IsNullOrEmpty(_username1) && !String.IsNullOrEmpty(_username2))
            {
                _battleStarted = true;
                List<string> battleResult = PlayBattle(_playerOneCards, _playerTwoCards, _username1,_username2, _userId1, _userId2);
                _lastBattleResult = battleResult;
            }
        }

       
        if (_lastBattleResult != null && _lastBattleResult.Count > 0)
        {
            foreach (string result in _lastBattleResult)
            {
                res += result + "\n";
                Console.WriteLine(result);
            }
        }
        else
        {
            res = "Error: Battle could not be initiated.";
        }

        return res;
    }
}
