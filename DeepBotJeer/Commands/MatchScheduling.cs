using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

#pragma warning disable CS1998

namespace SpaceBallsBot.Commands;

public class MatchScheduling : BaseCommandModule
{
    public static readonly HashSet<DiscordMember> RglRoster = new();
    public static readonly HashSet<DiscordMember> UgcRoster = new();

    [Command("ready_up")]
    public async Task ReadyUp(CommandContext ctx, double timeOut)
    {
        var message = await ctx.RespondAsync("React if you can make todays game");
        await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
        var reactions = await message.CollectReactionsAsync(TimeSpan.FromSeconds(timeOut));

        var reactors = new HashSet<DiscordUser>();
        foreach (var reaction in reactions)
        foreach (var user in reaction.Users)
            if (!user.IsBot)
                reactors.Add(user);

        var guildReactors = new HashSet<DiscordMember>();
        await UserToMember(ctx, reactors, guildReactors);

        var tagRole = await ctx.Guild.CreateRoleAsync("Ready", Permissions.None,
            DiscordColor.Green,
            false, true, "Readied up for gaming!");

        foreach (var member in guildReactors) await member.GrantRoleAsync(tagRole);
        await Task.Delay(60 * (int)timeOut);

        foreach (var member in guildReactors) await member.RevokeRoleAsync(tagRole);
        await tagRole.DeleteAsync();
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

    public static async Task UserToMember(CommandContext ctx, HashSet<DiscordUser> users,
        HashSet<DiscordMember> members)
    {
        foreach (var user in users) members.Add(await ctx.Guild.GetMemberAsync(user.Id));
    }
}