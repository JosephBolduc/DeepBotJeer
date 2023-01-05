using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace SpaceballsBot.Event_Handlers;

public class ScheduledGuildEventCreated
{
    public static HashSet<DiscordScheduledGuildEvent> EventList = new();
    private static DiscordClient? _discordClient;

    public static async Task Handler(DiscordClient s, ScheduledGuildEventCreateEventArgs e)
    {
        _discordClient = s;
        EventList.Add(e.Event);
    }

    public static void Initialize(DiscordClient s)
    {
        GetAllEvents(s);
        KickOffEventLoop(s);
    }

    private static void GetAllEvents(DiscordClient s)
    {
        foreach (var guild in s.Guilds)
        foreach (var scheduledEvent in guild.Value.ScheduledEvents)
            EventList.Add(scheduledEvent.Value);
    }

    private static void KickOffEventLoop(DiscordClient s)
    {
        _discordClient = s;
        _ = Task.Run(EventLoop);
    }

    private static void EventLoop()
    {
        while (true)
        {
            Thread.Sleep(10 * 60 * 1000);
            foreach (var scheduledEvent in EventList) Console.WriteLine(scheduledEvent.Name);
        }
    }
}