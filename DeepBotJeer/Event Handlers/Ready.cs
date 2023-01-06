using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace SpaceballsBot.Event_Handlers;

public class Ready
{
    public static async Task Handler(DiscordClient s, ReadyEventArgs e)
    {
        var discordActivity = new DiscordActivity("Weezer", ActivityType.ListeningTo);
        await s.UpdateStatusAsync(userStatus: UserStatus.Online, activity: discordActivity);
    }
}