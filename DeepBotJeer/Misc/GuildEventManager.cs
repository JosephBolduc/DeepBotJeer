using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using SpaceBallsBot;

namespace SpaceballsBot.Misc;

public static class GuildEventManager
{
    // dict is <eventID, eventOBJ>
    private static readonly ConcurrentDictionary<ulong, DiscordScheduledGuildEvent> runningEvents = new();

    // accepts an event object to process and run a task for if needed
    public static void ProcessEvent(DiscordScheduledGuildEvent guildEvent)
    {
        // Check if the event is related to TF2 scheduling
        if (!IsEventTf2(guildEvent)) return;

        // Makes sure the event hasn't already been processed
        if (runningEvents.ContainsKey(guildEvent.Id)) return;


        runningEvents.TryAdd(guildEvent.Id, guildEvent);
        Task.Run(() => EventRunner(guildEvent));
    }

    private static async Task EventRunner(DiscordScheduledGuildEvent guildEvent)
    {
        DiscordGuild guild = guildEvent.Guild;
        DiscordChannel targetChannel = FindAnnouncementsChannel(guild);
        GetTf2EventDetails(guildEvent, out bool isRgl, out bool isUgc, out bool isMatch);
        GetTf2ServerRoles(guild, out DiscordRole rglRole, out DiscordRole ugcRole);

        TimeSpan timeUntilEvent = guildEvent.StartTime - DateTimeOffset.UtcNow;

        string eventPings = "";
        if (isRgl) eventPings += rglRole.Mention;
        if (isRgl && isUgc) eventPings += "/";
        if (isUgc) eventPings += ugcRole.Mention;

        DiscordMessage notifyMessage = await targetChannel.SendMessageAsync(
            "Hey " + eventPings + ", theres going to be a " +
            (isMatch ? "match" : "scrim") + " in " + FormatTimeSpan(timeUntilEvent) +
            ". React any emoji to this message if you can make it");

        await notifyMessage.CreateReactionAsync(
            DiscordEmoji.FromName(Program.DiscordClient, ":thumbsup:"));

        string roleSuffix = guildEvent.Id.ToString()[(guildEvent.Id.ToString().Length - 8)..];

        DiscordRole? targetRole =
            guild.Roles.Values.FirstOrDefault(role => role.Name.ToLower().Equals("ready" + roleSuffix));

        if (targetRole == null)
            targetRole = await guild.CreateRoleAsync("ready-" + roleSuffix, Permissions.None,
                DiscordColor.Gold,
                false,
                true,
                "Why are you reading the audit logs you nerd redditor?");

        // Runs a task that assigns roles to the people who reacted to the notification message
        _ = Task.Run(async () =>
        {
            HashSet<DiscordMember> matchRoster =
                guild.Members.Values.Where(member => member.Roles.Contains(targetRole)).ToHashSet();

            while (DateTime.UtcNow < guildEvent.StartTime)
            {
                // Creates a new set for this rounds reactors
                HashSet<DiscordMember> guildReactors = new();
                ReadOnlyCollection<Reaction>? reactions =
                    await notifyMessage.CollectReactionsAsync(TimeSpan.FromSeconds(10));
                reactions.ToList().ForEach(reaction =>
                {
                    reaction.Users.ToList().ForEach(async user =>
                    {
                        if (user.IsBot) return;
                        DiscordMember member = await guild.GetMemberAsync(user.Id);

                        guildReactors.Add(member);
                        if (matchRoster.Add(member)) await member.GrantRoleAsync(targetRole);
                    });
                });
                matchRoster = matchRoster.Where(member =>
                {
                    if (guildReactors.Contains(member)) return true;
                    matchRoster.Remove(member);
                    member.RevokeRoleAsync(targetRole);
                    return false;
                }).ToHashSet();
            }
        });


        //TODO add feature that picks up where a crashed event thread left off
        switch (timeUntilEvent.TotalHours)
        {
            case >= 6:
                // six hour notification
                break;
            case >= 1:
                // one hour notification
                break;
            case >= .5:
                // half hour notification
                break;
        }
    }

    // Checks if the event is relevant to TF2 scheduling
    private static bool IsEventTf2(DiscordScheduledGuildEvent guildEvent)
    {
        string description = guildEvent.Description.ToLower();
        return (description.Contains("rgl") || description.Contains("ugc")) &&
               (description.Contains("match") || description.Contains("scrim"));
    }

    private static void GetTf2EventDetails(DiscordScheduledGuildEvent guildEvent, out bool isRgl, out bool isUgc,
        out bool isMatch)
    {
        string description = guildEvent.Description.ToLower();

        isRgl = false;
        isUgc = false;
        isMatch = false;

        if (description.Contains("rgl")) isRgl = true;
        if (description.Contains("ugc")) isUgc = true;
        if (description.Contains("match")) isMatch = true;
    }

    private static void GetTf2ServerRoles(DiscordGuild guild, out DiscordRole rglRole,
        out DiscordRole ugcRole)
    {
        rglRole = null!;
        ugcRole = null!;

        foreach (DiscordRole? role in guild.Roles.Values)
            switch (role.Name.ToLower())
            {
                case "rgl":
                    rglRole = role;
                    break;
                case "ugc":
                    ugcRole = role;
                    break;
            }
    }

    // Finds the general text channel in a guild, if any
    private static DiscordChannel FindAnnouncementsChannel(DiscordGuild guild)
    {
        DiscordChannel? announcements = null;
        DiscordChannel? general = null;

        foreach (DiscordChannel? channel in guild.Channels.Values)
            switch (channel.Name)
            {
                case "announcements":
                    announcements = channel;
                    break;
                case "general":
                    general = channel;
                    break;
            }

        if (announcements != null) return announcements;
        if (general != null) return general;
        return guild.GetDefaultChannel();
    }

    private static string FormatTimeSpan(TimeSpan span)
    {
        bool hasHours = (int)span.TotalHours != 0;
        bool hasMinutes = span.Minutes != 0;

        StringBuilder builder = new();

        if (hasHours)
        {
            builder.Append((int)span.TotalHours + " hour");
            if ((int)span.TotalHours != 1) builder.Append('s');
        }

        if (hasHours && hasMinutes) builder.Append(" and ");

        if (hasMinutes)
        {
            builder.Append(span.Minutes + " minute");
            if (span.Minutes != 1) builder.Append('s');
        }

        return builder.ToString();
    }

    // Unneeded alternative to using a GUID in the desc
    private static string HashText(string text)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(text)));
    }
}