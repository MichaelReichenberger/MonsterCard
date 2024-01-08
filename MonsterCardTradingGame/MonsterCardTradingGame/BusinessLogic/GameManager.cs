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

    private static Mapper _mapper;
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


    public GameManager()
    {
        _mapper = new Mapper();
        _lastBattleResult = null;
        _userRepository = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
        _statsRepository = new StatsRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
        _username1 = default;
        _username2 = default;
        _userId1 = default;
        _userId2 = default;
        _battleStarted = false;
        ResetGame();
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


    //Logic for fights
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int CardFight(Card playerOneCard, Card playerTwoCard)
    {

        //Booster of one card is active
        if (playerOneCard.CurrentWins >= 2 && playerTwoCard.CurrentWins < 2)
        {
            playerOneCard.CurrentWins = 0;
            return 1;
        }

        if (playerTwoCard.CurrentWins >= 2 && playerOneCard.CurrentWins < 2)
        {
            playerTwoCard.CurrentWins = 0;
            return 2;
        }

        //Goblin vs Dragon
        if (playerOneCard.Name == "Goblin" && playerTwoCard.Name == "Dragon")
        {
            playerTwoCard.CurrentWins++;
            return 2;
        }
        if (playerTwoCard.Name == "Goblin" && playerOneCard.Name == "Dragon")
        {
            playerOneCard.CurrentWins++;
            return 1;
        }

        //Wizard vs Ork
        if (playerOneCard.Name == "Wizard" && playerTwoCard.Name == "Ork")
        {
            playerOneCard.CurrentWins++;
            return 1;
        }
        if (playerTwoCard.Name == "Wizard" && playerOneCard.Name == "Ork")
        {
            playerTwoCard.CurrentWins++;
            return 2;
        }

        //Knight vs  WaterSpell
        if (playerOneCard.Name == "Knight" && playerTwoCard.Element == Element.Water && playerTwoCard.Name == "Spell")
        {
            playerTwoCard.CurrentWins++;
            return 2;
        }

        if (playerTwoCard.Name == "Knight" && playerOneCard.Element == Element.Water && playerOneCard.Name == "Spell")
        {
            playerOneCard.CurrentWins++;
            return 1;
        }


        //Dragon vs Elve
        if (playerOneCard.Name == "Dragon" && playerTwoCard.Element == Element.Fire && playerTwoCard.Name == "Elve")
        {
            playerTwoCard.CurrentWins++;
            return 2;
        }

        if (playerTwoCard.Name == "Elve" && playerTwoCard.Element == Element.Fire && playerOneCard.Name == "Dragon")
        {
            playerOneCard.CurrentWins++;
            return 1;
        }


        double damagePlayerOne = playerOneCard.Damage;
        double damagePlayerTwo = playerTwoCard.Damage;


        //If One Card is a Spell
        if (playerOneCard.Type == "Spell" || playerTwoCard.Type == "Spell" || (playerOneCard.Type == "Spell" && playerTwoCard.Type == "Spell"))
        {
            if (playerOneCard.Name != "Kraken" && playerTwoCard.Name != "Kraken")
            {
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
            }

            //Kraken vs Spell
            else if (playerOneCard.Name == "Kraken" && playerTwoCard.Name == "Spell")
            {
                playerOneCard.CurrentWins++;
                return 1;
            }
            else if (playerTwoCard.Name == "Kraken" && playerOneCard.Name == "Spell")
            {
                playerTwoCard.CurrentWins++;
                return 2;
            }
        }

        //If both cards are monsters
        if (damagePlayerOne > damagePlayerTwo)
        {
            playerOneCard.CurrentWins++;
            return 1;
        }

        if (damagePlayerTwo > damagePlayerOne)
        {
            playerTwoCard.CurrentWins++;
            return 2;
        }
        return 0;
    }


    //Play the battle
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public List<string> PlayBattle(CardDeck player1Deck, CardDeck player2Deck, string username1, string username2, int userId1, int userId2)
    {
        //Lock the object to prevent other threads from accessing it
        lock (lockObject)
        {

            List<string> battleResults = new List<string>();

            //Inialize random number generator
            Random rand = new Random();
            string winner;
            int rounds = 0;

            //While playerdecks are not empty and the game is not longer than 100 rounds play the game
            while (player1Deck.Deck.Count > 0 && player2Deck.Deck.Count > 0 && rounds <= 100)
            {
                Console.WriteLine(username1);
                Console.WriteLine(username2);

                //Get random nunmbers
                int i = rand.Next(player1Deck.Deck.Count);
                int j = rand.Next(player2Deck.Deck.Count);


                //Ensure that the index is not out of bounds
                //Execute Fight
                if (player1Deck.Deck.Count > i && player2Deck.Deck.Count > j)
                {
                    Card player1Card = player1Deck.Deck[i];
                    Card player2Card = player2Deck.Deck[j];
                    int fightResult = CardFight(player1Card, player2Card);

                    if (fightResult == 1)
                    {
                        battleResults.Add($"{username1} wins with {player1Card.Name} against {player2Card.Name}");
                        player1Deck.Deck.Add(player2Card);
                        player2Deck.Deck.Remove(player2Card);
                    }
                    else if (fightResult == 2)
                    {
                        battleResults.Add($"{username2} wins with {player2Card.Name} against {player1Card.Name}");
                        player2Deck.Deck.Add(player1Card);
                        player1Deck.Deck.Remove(player1Card);
                    }
                    else
                    {
                        battleResults.Add($"Draw between {player1Card.Name} and {player2Card.Name}");
                    }
                }
                rounds++;
            }

            //If PlayerTwo wins
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
            //If PlayerOne wins
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
            //If Draw happens
            else
            {
                battleResults.Add(
                    $"Draw-Current score: {username1} {player1Deck.Deck.Count} - {player2Deck.Deck.Count} {username2}");
            }
            //ResetGame for next battle
            ResetGame();
            _lastBattleResult = battleResults;
            return battleResults;
        }
    }

    //Wait for another player to join the game and start the battle
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public List<string> WaitForOtherPlayerAndStartBattle(int userId)
    {
        Console.WriteLine($"User with ID {userId} is attempting to join the game");
        string res = "";


        lock (lockObject)
        {
            playerCount++;
            Console.WriteLine($"Current player count: {playerCount}");

            //If there is only one player, wait for another player to join
            if (playerCount == 1)
            {
                Console.WriteLine("User 1 joined, waiting for another player...");
                _playerOneCards = _mapper.ParseCardDeck(userId);
                _username1 = _userRepository.GetName(userId);
                _userId1 = userId;
                Console.WriteLine(_username1);
                Monitor.Wait(lockObject);
            }
            //If there are two players, start the battle
            else if (playerCount == 2)
            {
                Console.WriteLine("User 2 joined, starting the battle...");
                _playerTwoCards = _mapper.ParseCardDeck(userId);
                _username2 = _userRepository.GetName(userId);
                _userId2 = userId;
                Console.WriteLine(_username2);
                Monitor.Pulse(lockObject);
            }

            
            if (_battleStarted == false && _playerOneCards != null && _playerTwoCards != null
                && !String.IsNullOrEmpty(_username1) && !String.IsNullOrEmpty(_username2))
            {
                _battleStarted = true;
                //Start the battle
                List<string> battleResult = PlayBattle(_playerOneCards, _playerTwoCards, _username1, _username2, _userId1, _userId2);
                _lastBattleResult = battleResult;
            }
        }
        return _lastBattleResult;
    }


    //Reset the state of the game
    private void ResetGame()
    {
        _battleStarted = false;
        playerCount = 0;
        _lastBattleResult = new List<string>();
        _username1 = default;
        _username2 = default;
        _userId1 = default;
        _userId2 = default;
        _playerOneCards = null;
        _playerTwoCards = null;
    }
}