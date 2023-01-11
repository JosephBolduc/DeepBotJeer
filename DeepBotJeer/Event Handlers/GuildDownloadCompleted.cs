using DSharpPlus;
using DSharpPlus.EventArgs;
using SpaceBallsBot.Commands;
using SpaceballsBot.Misc;

#pragma warning disable CS1998

namespace SpaceballsBot.Event_Handlers;

public class GuildDownloadCompleted
{
    public static async Task Handler(DiscordClient s, GuildDownloadCompletedEventArgs e)
    {
        MatchScheduling.LoadRoster(s);
        e.Guilds.Values.ToList().ForEach(guild =>
        {
            guild.ScheduledEvents.Values.ToList().ForEach(GuildEventManager.ProcessEvent);
        });
        //GuildEventModule.KickOffEventLoop(s);
    }
}