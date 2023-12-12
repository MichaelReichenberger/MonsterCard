public class GameManager
{
    private SemaphoreSlim semaphore = new SemaphoreSlim(0, 2); // Anzahl der Spieler
    private int? sharedRandomNumber;

    private static GameManager instance = null;
    private static readonly object lockObject = new object();

    private GameManager()
    {
    }

    public static GameManager Instance
    {
        get
        {
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }
    }

    public async Task<string> WaitForOtherPlayerAndStartBattleAsync()
    {
        Console.WriteLine("Waiting for another player...");
        await semaphore.WaitAsync(); // Wartet, bis ein anderer Spieler die Route aufruft

        if (!sharedRandomNumber.HasValue)
        {
            Random rnd = new Random();
            sharedRandomNumber = rnd.Next();
            semaphore.Release(2); // Freigabe für beide Spieler, um fortzufahren
        }
        else
        {
            semaphore.Release(); // Erlaubt einem zusätzlichen Spieler, fortzufahren
        }

        Console.WriteLine($"Shared number: {sharedRandomNumber}");
        return sharedRandomNumber.ToString();
    }
}