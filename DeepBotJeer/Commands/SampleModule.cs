using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace SpaceBallsBot.Commands;

public class SampleModule : BaseCommandModule
{
    [Command("status")]
    [Aliases("test")]
    public async Task Status(CommandContext ctx)
    {
        /*var builder = new StringBuilder();
        var assembly = Assembly.GetExecutingAssembly();
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

        builder.AppendJoin(" ", "Currently running", fileVersionInfo.ProductName, "version",
            fileVersionInfo.ProductVersion, "on", Environment.MachineName);
        await ctx.RespondAsync(builder.ToString());*/
        await ctx.RespondAsync("running!");
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