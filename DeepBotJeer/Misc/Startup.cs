namespace SpaceballsBot.Misc;

public static class Startup
{
    private const string TokenFile = "credentials.txt";

    // Called by Main to create an empty token file if none and start garbage collection loop
    public static void Initialize()
    {
        if (!File.Exists(TokenFile)) File.Create(TokenFile);
        KickOffGCL();
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

    // Starts the garbage collection loop in a new thread
    private static void KickOffGCL()
    {
        _ = Task.Run(GarbageCollectionLoop);
    }

    // Constantly runs the garbage collector because of Raspi memory issues
    private static void GarbageCollectionLoop()
    {
        while (true)
        {
            Thread.Sleep(60 * 1000);
            GC.Collect();
            // Console.WriteLine("Ran GC");
        }
    }
}