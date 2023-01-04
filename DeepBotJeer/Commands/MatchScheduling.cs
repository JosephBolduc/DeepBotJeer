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

    [Command("load_roster")]
    [Aliases("update_roster", "reload_roster")]
    [RequireOwner]
    public async Task LoadRoster(CommandContext ctx)
    {
        LoadRoster(ctx.Client.Guilds);
    }

    public static void LoadRoster(DiscordClient client)
    {
        LoadRoster(client.Guilds);
    }

    private static void LoadRoster(IReadOnlyDictionary<ulong, DiscordGuild> clientGuilds)
    {
        foreach (var guild in clientGuilds)
        foreach (var member in guild.Value.Members)
        foreach (var role in member.Value.Roles)
            switch (role.Name)
            {
                case "RGL":
                    RglRoster.Add(member.Value);
                    break;
                case "UGC":
                    UgcRoster.Add(member.Value);
                    break;
            }

        Console.WriteLine("Loaded TF2 rosters");
    }
}