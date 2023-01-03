using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DeepBotJeer;

public static class MessageCreated
{
    public static async Task Handler(DiscordClient s, MessageCreateEventArgs e)
    {
        if (e.Author.IsBot) return;

        ChatReactions.Handler(s, e);

        if (e.Message.Content.ToLower().StartsWith("ping"))
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000);
                await e.Message.RespondAsync("bonk!");
            });

        if (e.Message.Content.Equals("<@1058823504776134696> what have you done."))
            _ = Task.Run(async () =>
            {
                await Task.Delay(500);
                await e.Message.RespondAsync("uhhh, nothing");
                await Task.Delay(5000);
                await e.Message.RespondAsync("thank you for your understanding");
            });

        Console.WriteLine(e.Message.Content);
    }
}