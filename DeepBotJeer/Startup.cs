namespace DeepBotJeer;

public static class Startup
{
    private static readonly string TokenFile = "credentials.txt";

    public static void Initialize()
    {
        if (!File.Exists(TokenFile)) File.Create(TokenFile);
        KickOffGCL();
    }

    public static string GetToken()
    {
        var token = GetTokenFromFile();
        if (token != "") return token;

        token = GetTokenFromEnv();
        return token;
    }

    private static string GetTokenFromFile()
    {
        Console.WriteLine("Reading token from file");
        var fileText = File.ReadAllText(TokenFile);
        return fileText;
    }

    private static string GetTokenFromEnv()
    {
        // TODO Add ability to get token from env variable
        return "";
    }

    private static void KickOffGCL()
    {
        _ = Task.Run(GarbageCollectionLoop);
    }

    private static void GarbageCollectionLoop()
    {
        while (true)
        {
            Thread.Sleep(60 * 1000);
            Console.WriteLine("Ran GC");
            GC.Collect();
        }
    }
}