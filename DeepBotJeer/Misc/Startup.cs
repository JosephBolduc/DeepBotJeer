namespace SpaceballsBot.Misc;

public static class Startup
{
    private const string TokenFile = "credentials.txt";

    // Called by Main to create an empty token file if none and start garbage collection loop
    public static void Initialize()
    {
        if (!File.Exists(TokenFile)) File.Create(TokenFile);
        Task.Run(GarbageCollectionLoop);
    }

    // Collects the bot token from either a file or environment var
    public static string GetToken()
    {
        string token = GetTokenFromFile();
        if (token != "") return token;

        token = GetTokenFromEnv();
        return token;
    }

    private static string GetTokenFromFile()
    {
        Console.WriteLine("Reading token from file");
        string fileText = File.ReadAllText(TokenFile);
        return fileText;
    }

    private static string GetTokenFromEnv()
    {
        // TODO Add ability to get token from env variable
        return "";
    }

    // Constantly runs the garbage collector because of Raspi memory issues
    // Maybe it isn't needed anymore but idk
    private static async Task GarbageCollectionLoop()
    {
        while (true)
        {
            await Task.Delay(5 * 60 * 1000);
            GC.Collect();
            // Console.WriteLine("Ran GC");
        }
    }
}