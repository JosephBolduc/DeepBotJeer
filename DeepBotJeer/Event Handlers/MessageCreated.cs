using DSharpPlus;
using DSharpPlus.EventArgs;
using SpaceballsBot.Misc;

#pragma warning disable CS1998

namespace SpaceballsBot.Event_Handlers;

public static class MessageCreated
{
    public static async Task Handler(DiscordClient s, MessageCreateEventArgs e)
    {
        if (e.Author.IsBot) return;

        ChatReactions.Handler(s, e);

        /*var builder = new StringBuilder();
        builder.Append(e.Guild.Name + "::");
        builder.Append(e.Channel.Name + "::");
        builder.Append(e.Author.Username + "::");
        builder.Append(e.Message.Content);
        Console.WriteLine(builder.ToString());*/
    }
}