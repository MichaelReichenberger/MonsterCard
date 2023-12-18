﻿public class GameManager
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