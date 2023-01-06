using DSharpPlus;
using DSharpPlus.EventArgs;
using SpaceBallsBot.Commands;

#pragma warning disable CS1998

namespace SpaceballsBot.Event_Handlers;

public class GuildDownloadCompleted
{
    public static async Task Handler(DiscordClient s, GuildDownloadCompletedEventArgs e)
    {
        MatchScheduling.LoadRoster(s);
        GuildEventModule.LoadEvents(s);
        GuildEventModule.KickOffEventLoop(s);
    }
}