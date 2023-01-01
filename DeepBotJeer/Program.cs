using DSharpPlus;

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

            discord.MessageCreated += async (s, e) =>
            {
                if (!e.Author.IsBot)
                {
                    if (e.Message.Content.ToLower().StartsWith("ping"))
                    {
                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(3000);
                            await e.Message.RespondAsync("bonk!");
                        });
                    }

                    if (e.Message.Content.Equals("<@1058823504776134696> what have you done."))
                    {
                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(500);
                            await e.Message.RespondAsync("uhhh, nothing");
                            await Task.Delay(5000);
                            await e.Message.RespondAsync("thank you for your understanding");
                        });
                    }
                    Console.WriteLine(e.Message.Content);
                }
            }; 
            
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
        
    }
};