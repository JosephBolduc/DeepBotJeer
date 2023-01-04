using DSharpPlus;
using DSharpPlus.CommandsNext;
using SpaceBallsBot.Commands;
using SpaceballsBot.Event_Handlers;

namespace SpaceBallsBot;

internal static class Program
{
    private static void Main(string[] arfs)
    {
        Startup.Initialize();
        var token = Startup.GetToken();
        MainAsync(token).GetAwaiter().GetResult();
    }

    private static async Task MainAsync(string token)
    {
        var discord = new DiscordClient(new DiscordConfiguration
        {
            Token = token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All
        });

        var commands = discord.UseCommandsNext(new CommandsNextConfiguration
        {
            StringPrefixes = new[] { "tf_" }
        });

        commands.RegisterCommands<SampleModule>();
        commands.RegisterCommands<MatchScheduling>();

        discord.GuildDownloadCompleted += GuildDownloadCompleted.Handler;
        discord.MessageCreated += MessageCreated.Handler;

        await discord.ConnectAsync();

        await Task.Delay(-1);
    }
}