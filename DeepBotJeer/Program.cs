using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using SpaceBallsBot.Commands;
using SpaceballsBot.Event_Handlers;
using SpaceballsBot.Misc;

namespace SpaceBallsBot;

internal static class Program
{
    public static DiscordClient? DiscordClient;

    private static void Main(string[] arfs)
    {
        // var assembly = Assembly.GetExecutingAssembly();
        // Console.Title = assembly.FullName;

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
            StringPrefixes = new[] { "tfdev_" },
            CaseSensitive = false
        });

        discord.UseInteractivity(new InteractivityConfiguration
        {
            PollBehaviour = PollBehaviour.KeepEmojis,
            Timeout = TimeSpan.FromSeconds(20)
        });

        DiscordClient = discord;

        commands.RegisterCommands<SampleModule>();
        commands.RegisterCommands<GuildEventModule>();
        commands.RegisterCommands<MatchScheduling>();

        discord.Ready += Ready.Handler;
        discord.GuildDownloadCompleted += GuildDownloadCompleted.Handler;
        discord.MessageCreated += MessageCreated.Handler;
        discord.ClientErrored += ClientErrored.Handler;
        discord.ScheduledGuildEventCreated += ScheduledGuildEventCreated.Handler;
        discord.ScheduledGuildEventUpdated += ScheduledGuildEventUpdated.Handler;


        await discord.ConnectAsync();

        await Task.Delay(-1);
    }
}