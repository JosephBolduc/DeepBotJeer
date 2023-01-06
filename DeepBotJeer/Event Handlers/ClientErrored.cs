using DSharpPlus;
using DSharpPlus.EventArgs;

#pragma warning disable CS1998

namespace SpaceballsBot.Event_Handlers;

public class ClientErrored
{
    public static async Task Handler(DiscordClient s, ClientErrorEventArgs e)
    {
        var consoleColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e.Exception.Message);
        Console.ForegroundColor = consoleColor;
    }
}