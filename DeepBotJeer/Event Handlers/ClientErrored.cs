using DSharpPlus;
using DSharpPlus.EventArgs;

namespace SpaceballsBot.Event_Handlers;

public class ClientErrored
{
    public static async Task Handler(DiscordClient s, ClientErrorEventArgs e)
    {
        Console.WriteLine(e.Exception.Message);
    }
}