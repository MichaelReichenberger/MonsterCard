public class GameManager
{
    private int playerCount = 0;
    private int? sharedRandomNumber;
    private readonly object lockObject = new object();

    private static GameManager instance = null;
    private static readonly object instanceLockObject = new object();

    private GameManager()
    {
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

    public async Task<string> WaitForOtherPlayerAndStartBattleAsync()
    {
        Console.WriteLine("Waiting for another player...");

        lock (lockObject)
        {
            playerCount++;
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

        // Warten auf den zweiten Spieler
        while (true)
        {
            await Task.Delay(100); // Kurze Verzögerung, um die CPU nicht zu blockieren
            lock (lockObject)
            {
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
        }
    }
}