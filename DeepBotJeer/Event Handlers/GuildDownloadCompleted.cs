using DSharpPlus;
using DSharpPlus.EventArgs;
using SpaceBallsBot.Commands;

namespace SpaceballsBot.Event_Handlers;

public class GuildDownloadCompleted
{
    public static async Task Handler(DiscordClient s, GuildDownloadCompletedEventArgs e)
    {
        MatchScheduling.LoadRosters(s);
        Console.WriteLine("Loaded TF2 rosters");
    }
}