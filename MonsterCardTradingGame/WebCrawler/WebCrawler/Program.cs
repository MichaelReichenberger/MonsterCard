using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

class WebCrawler
{
    private static readonly HttpClient client = new HttpClient();
    private readonly string startUrl;
    private readonly int maxDepth;
    private ConcurrentDictionary<string, bool> visited = new ConcurrentDictionary<string, bool>();
    private int totalProcessed = 0;
    private Stopwatch stopwatch = new Stopwatch();

    public WebCrawler(string startUrl, int maxDepth)
    {
        this.startUrl = startUrl;
        this.maxDepth = maxDepth;
    }

    public async Task StartCrawlingAsync()
    {
        Console.WriteLine("Start Crawling");
        stopwatch.Start();
        await ProcessUrlAsync(startUrl, 0);
        stopwatch.Stop();
        Console.WriteLine($"Crawling completed. Total processed: {totalProcessed}, Duration: {stopwatch.Elapsed}");
        Console.WriteLine($"Unique links found: {visited.Count}");
    }

    private async Task ProcessUrlAsync(string url, int depth)
    {
        Console.WriteLine($"Processing URL at depth {depth}: {url}");
        if (depth > maxDepth || !visited.TryAdd(url, true))
        {
            Console.WriteLine($"Skipping URL (depth or already visited): {url}");
            return;
        }

        try
        {
            string html = await client.GetStringAsync(url);
            var tasks = new List<Task>();
            foreach (var link in ExtractLinks(html))
            {
                Console.WriteLine($"Found link: {link}");
                if (tasks.Count >= 10) // Begrenzen der Anzahl gleichzeitiger Aufgaben
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(t => t.IsCompleted);
                }
                tasks.Add(ProcessUrlAsync(link, depth + 1));
            }
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing {url}: {ex.Message}");
        }
        finally
        {
            Interlocked.Increment(ref totalProcessed);
            Console.WriteLine($"Completed processing URL: {url}");
        }
    
    private IEnumerable<string> ExtractLinks(string html)
    {
        var matches = Regex.Matches(html, @"<a href=""(.*?)""");
        foreach (Match match in matches)
        {
            yield return match.Groups[1].Value;
        }
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        var crawler = new WebCrawler("https://moodle.technikum-wien.at/my/", 2);
        await crawler.StartCrawlingAsync();
    }
}
