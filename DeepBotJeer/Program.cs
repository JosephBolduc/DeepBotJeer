using DSharpPlus;
using DSharpPlus.Entities;

namespace DeepBotJeer
{
    static class Program
    {
        private static string GetDiscordToken()
        {
            try
            {
                Console.WriteLine("Reading token from file");
                string fileText = File.ReadAllText("credentials.txt");
                return fileText;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("credentials.txt not found");
                throw;
                // TODO Add ability to get token from env variable
            }
        }

        private static void Main(string[] arfs)
        {
            string token = GetDiscordToken();
            MainAsync(token).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string token)
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });

            discord.MessageCreated += MessageCreated.Handler;

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
};