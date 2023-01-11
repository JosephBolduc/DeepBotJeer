using DSharpPlus;
using DSharpPlus.EventArgs;

#pragma warning disable CS1998

namespace SpaceballsBot.Event_Handlers;

public class ScheduledGuildEventUpdated
{
    public static async Task Handler(DiscordClient s, ScheduledGuildEventUpdateEventArgs e)
    {
        // GuildEventModule.AddEvent(e.EventAfter);
    }
}