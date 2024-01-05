using MonsterCardTradingGame.BusinessLogic;
using MonsterCardTradingGame.Models;

public class GameManager
{
    private int playerCount = 0;
    private int? sharedRandomNumber;
    private readonly object lockObject = new object();
    private List<Card> _playerOneCards;
    private List<Card> _playerTwoCards;
    private static GameManager instance = null;
    private static readonly object instanceLockObject = new object();
    private static OR_Mapper _orMapper;
    private GameManager()
    {
        _orMapper = new OR_Mapper();
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

    public async Task<string> WaitForOtherPlayerAndStartBattleAsync(int userId)
    {
        Console.WriteLine(userId);
        _orMapper.ParseCardDeck(userId);
        lock (lockObject)
        {
            playerCount++;
            if (playerCount == 1)
            {
                Console.WriteLine("Waiting for another player...");
            }
            if (playerCount >= 2)
            {
                if (!sharedRandomNumber.HasValue)
                {
                    Random rnd = new Random();
                    sharedRandomNumber = rnd.Next();
                }
                Console.WriteLine($"Shared number: {sharedRandomNumber}");
                return sharedRandomNumber.ToString();
            }
        }

        //Wait for second player
        while (true)
        {
            await Task.Delay(10);
            lock (lockObject)
            {
                if (playerCount == 2)
                {
                    if (!sharedRandomNumber.HasValue)
                    {
                        Random rnd = new Random();
                        sharedRandomNumber = rnd.Next();
                    }
                    Console.WriteLine($"Shared number: {sharedRandomNumber}");
                    return sharedRandomNumber.ToString();
                }
            }
        }
    }
}