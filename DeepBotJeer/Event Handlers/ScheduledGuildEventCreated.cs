using DSharpPlus;
using DSharpPlus.EventArgs;
using SpaceBallsBot.Commands;

#pragma warning disable CS1998

namespace SpaceballsBot.Event_Handlers;

public class ScheduledGuildEventCreated
{
    public static async Task Handler(DiscordClient s, ScheduledGuildEventCreateEventArgs e)
    {
        GuildEventModule.AddEvent(e.Event);
    }
}