using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace SpaceballsBot.Event_Handlers;

public class Ready
{
    public static async Task Handler(DiscordClient s, ReadyEventArgs e)
    {
        var discordActivity = new DiscordActivity("JetBrains Rider 2022.3.1", ActivityType.Watching);
        await s.UpdateStatusAsync(userStatus: UserStatus.DoNotDisturb, activity: discordActivity);
    }
}