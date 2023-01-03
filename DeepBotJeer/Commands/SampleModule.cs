using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace SpaceBallsBot.Commands;

public class SampleModule : BaseCommandModule
{
    [Command("test")]
    public async Task GreetCommand(CommandContext ctx)
    {
        await ctx.RespondAsync("command executed");
    }

    [Command("say")]
    [RequireOwner]
    public async Task Say(CommandContext ctx, DiscordChannel channel, params string[] content)
    {
        var builder = new StringBuilder();
        builder.AppendJoin(' ', content);
        await channel.SendMessageAsync(builder.ToString());
    }
}