using DSharpPlus;
using DSharpPlus.EventArgs;
using SpaceBallsBot.Commands;

namespace SpaceballsBot.Event_Handlers;

public class GuildDownloadCompleted
{
    public static async Task Handler(DiscordClient s, GuildDownloadCompletedEventArgs e)
    {
        MatchScheduling.LoadRoster(s);
    }
}