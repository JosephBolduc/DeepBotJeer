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

    private static async Task Main()
    {
        Startup.Initialize();
        string token = Startup.GetToken();

        DiscordClient discord = new(new DiscordConfiguration
        {
            Token = token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All,
        });

        CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration
        {
            StringPrefixes = new[] { "tfprod" },
            CaseSensitive = false,
        });

        discord.UseInteractivity(new InteractivityConfiguration
        {
            PollBehaviour = PollBehaviour.KeepEmojis,
            Timeout = TimeSpan.FromSeconds(20),
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