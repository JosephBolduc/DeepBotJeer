using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace SpaceballsBot.Event_Handlers;

public class Ready
{
    public static async Task Handler(DiscordClient s, ReadyEventArgs e)
    {
        DiscordActivity discordActivity = new("Umineko no Naku Koro Ni", ActivityType.Watching);
        await s.UpdateStatusAsync(userStatus: UserStatus.Online, activity: discordActivity);
    }
}