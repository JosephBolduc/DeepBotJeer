using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace SpaceBallsBot.Commands;

public class MatchScheduling : BaseCommandModule
{
    private static readonly HashSet<DiscordMember> RglRoster = new();
    private static readonly HashSet<DiscordMember> UgcRoster = new();

    [Command("load_roster")]
    public async Task LoadRosters(CommandContext ctx)
    {
        foreach (var guild in ctx.Client.Guilds)
        foreach (var member in guild.Value.Members)
        foreach (var role in member.Value.Roles)
        {
            if (role.Name is "RGL") RglRoster.Add(member.Value);
            if (role.Name is "UGC") UgcRoster.Add(member.Value);
        }
    }

    public static void LoadRosters(DiscordClient d)
    {
        foreach (var guild in d.Guilds)
        foreach (var member in guild.Value.Members)
        foreach (var role in member.Value.Roles)
        {
            if (role.Name is "RGL") RglRoster.Add(member.Value);
            if (role.Name is "UGC") UgcRoster.Add(member.Value);
        }
    }

    [Command("list_roster")]
    public async Task ListRoster(CommandContext ctx)
    {
        var builder = new StringBuilder();
        builder.AppendLine("RGL Roster:");
        foreach (var player in RglRoster) builder.AppendLine(player.DisplayName);
        await ctx.RespondAsync(builder.ToString());
        builder.Clear();
        builder.AppendLine("UGC Roster:");
        foreach (var player in UgcRoster) builder.AppendLine(player.DisplayName);
        await ctx.Channel.SendMessageAsync(builder.ToString());
    }
}