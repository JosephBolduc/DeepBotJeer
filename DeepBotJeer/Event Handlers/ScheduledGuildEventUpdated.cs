using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SpaceBallsBot.Commands;

#pragma warning disable CS1998

namespace SpaceballsBot.Event_Handlers;

public class ScheduledGuildEventUpdated
{
    private static readonly HashSet<DiscordScheduledGuildEvent> EventList = GuildEventModule.EventList;

    public static async Task Handler(DiscordClient s, ScheduledGuildEventUpdateEventArgs e)
    {
        // GuildEventModule.AddEvent(e.EventAfter);
    }
}