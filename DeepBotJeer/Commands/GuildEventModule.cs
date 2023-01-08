using System.Collections.ObjectModel;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Net.Models;

namespace SpaceBallsBot.Commands;

public class GuildEventModule : BaseCommandModule
{
    public static readonly HashSet<DiscordScheduledGuildEvent> EventList = new();

    [Command("list_events")]
    public async Task ListEvents(CommandContext ctx)
    {
        StringBuilder builder = new();
        foreach (DiscordScheduledGuildEvent guildEvent in EventList)
            builder.AppendLine(guildEvent.Guild.Name + " - " + guildEvent.Name + " - " + guildEvent.Description);
        await ctx.RespondAsync(builder.ToString());
    }

    [Command("load_events")]
    public async Task LoadEvents(CommandContext ctx)
    {
        int eventCount = EventList.Count;
        LoadEvents(ctx.Client);
        StringBuilder builder = new();
        builder.AppendLine("Added " + eventCount + " new items");
        builder.AppendLine("Currently tracking " + EventList.Count + " items");
        await ctx.RespondAsync(builder.ToString());
    }

    // Takes a DiscordClient object, finds all relevant events, and loads them into EventList (actually a set but whatever)
    public static async void LoadEvents(DiscordClient s)
    {
        foreach (DiscordGuild? guild in s.Guilds.Values)
            foreach (DiscordScheduledGuildEvent? scheduledEvent in await guild.GetEventsAsync())
                AddEvent(scheduledEvent);
        Console.WriteLine("Loaded guild events");
    }

    // The definitive method to add events to the array
    public static void AddEvent(DiscordScheduledGuildEvent guildEvent)
    {
        if (!EventFilter(guildEvent)) return;
        foreach (DiscordScheduledGuildEvent item in EventList)
        {
            if (guildEvent.Id != item.Id) continue;
            EventList.Remove(item);
            return;
        }

        EventList.Add(guildEvent);
    }

    // Makes sure that the supplied event is relevant to TF2 scrims/matches
    // Event description must contain "scrim" or "match" as well as "rgl" or "ugc"
    private static bool EventFilter(DiscordScheduledGuildEvent guildEvent)
    {
        // Inner function to add a GUID to the description (if needed)
        // Appends "ID: {GUID}"
        void GuidStampEvent(ScheduledGuildEventEditModel model)
        {
            // Prevents double stamping
            if (guildEvent.Description.Contains("ID: {")) return;

            Console.WriteLine("Stamping " + guildEvent.Name);
            StringBuilder builder = new();
            builder.AppendLine(guildEvent.Description);
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("ID: {" + Guid.NewGuid().ToString()[..8] + "}");

            model.Description = builder.ToString();
        }

        string? title = guildEvent.Name;
        string? description = guildEvent.Description;
        string fullEventText = title + description;
        fullEventText = fullEventText.ToLower();

        if (!fullEventText.Contains("rgl") && !fullEventText.Contains("ugc")) return false;
        if (fullEventText.Contains("scrim") || fullEventText.Contains("match"))
        {
            guildEvent.Guild.ModifyEventAsync(guildEvent, GuidStampEvent, "Added GUID for identification");
            return true;
        }

        return false;
    }


    private static string ExtractId(DiscordScheduledGuildEvent guildEvent)
    {
        try
        {
            string? description = guildEvent.Description;
            description = description.Substring(description.IndexOf("ID: {"));
            description = description.Substring(5, 8);
            return description;
        }
        catch (Exception e)
        {
            string? description = Program.DiscordClient.Guilds[guildEvent.GuildId].ScheduledEvents[guildEvent.Id]
                .Description;
            description = description.Substring(description.IndexOf("ID: {"));
            description = description.Substring(5, 8);
            return description;
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


    // Starts the event loop in a new thread
    // Only call once!!
    public static void KickOffEventLoop(DiscordClient s)
    {
        _ = Task.Run(EventLoop);
    }

    // Kicks off a thread for each tracked event when it is time
    // Runs every 5 minutes
    private static async Task EventLoop()
    {
        HashSet<ulong> activeEventIds = new();
        const double waitMinutes = 6;
        const int hoursUntilNotify = 6;
        while (true)
        {
            await Task.Delay((int)(waitMinutes * 60 * 1000));
            Console.WriteLine("Ran event loop");

            foreach (DiscordScheduledGuildEvent guildEvent in EventList)
            {
                DateTime startTime = guildEvent.StartTime.DateTime;
                TimeSpan timeDiff = startTime - DateTime.UtcNow;

                // Kicks off thread if there less than 6 hours 'til and its ID isn't in the set
                if (timeDiff.TotalHours < hoursUntilNotify && !activeEventIds.Contains(guildEvent.Id))
                {
                    activeEventIds.Add(guildEvent.Id);
                    Console.WriteLine(guildEvent.Id);
                    _ = Task.Run(() => IndividualEventChecker(guildEvent));
                }
            }
        }
    }

    private static async Task IndividualEventChecker(DiscordScheduledGuildEvent guildEvent)
    {
        DateTime startTime = guildEvent.StartTime.DateTime;
        TimeSpan timeDiff = startTime - DateTime.UtcNow;

        // How long before the event to ping people
        TimeSpan timeUntilEvent = TimeSpan.FromMinutes(30);

        // How long after the initial ping to wait for emoji reactions and the @ready-GUID ping
        TimeSpan timeBeforeEvent = timeDiff - timeUntilEvent;

        // How long after the game starts to delete the temporary roles
        TimeSpan timeAfterStart = TimeSpan.FromMinutes(30);

        string id = ExtractId(guildEvent);

        DiscordRole? targetRole = null;
        DiscordRole? rglRole = null;
        DiscordRole? ugcRole = null;

        foreach (DiscordRole? role in guildEvent.Guild.Roles.Values)
        {
            switch (role.Name.ToLower())
            {
                case "rgl":
                    rglRole = role;
                    break;
                case "ugc":
                    ugcRole = role;
                    break;
            }

            if (role.Name.Equals("ready-" + id)) targetRole = role;
        }


        bool roleCreated = false;
        if (targetRole != null)
        {
            Console.WriteLine("Picked up unfinished event, continuing thread...");
            roleCreated = true;
        }

        StringBuilder builder = new();
        DiscordChannel general = FindAnnouncementsChannel(guildEvent.Guild);
        if (!roleCreated)
        {
            targetRole = await guildEvent.Guild.CreateRoleAsync("ready-" + id, Permissions.None, DiscordColor.Gold,
                false,
                true,
                "Why are you reading the audit logs you nerd redditor?");


            builder.Append("Hey ");
            if (guildEvent.Description.ToLower().Contains("rgl")) builder.Append(rglRole?.Mention);
            if (guildEvent.Description.ToLower().Contains("ugc")) builder.Append(ugcRole?.Mention);
            builder.Append(", theres going to be a ");
            builder.Append(guildEvent.Description.ToLower().Contains("match") ? "match " : "scrim ");
            builder.AppendLine("in about " + timeDiff.TotalHours.ToString()[..5] + " hours");
            builder.AppendLine("If you can make it, react any emoji to this message");


            DiscordMessage? reactionMessage = await general.SendMessageAsync(builder.ToString());
            builder.Clear();
            await reactionMessage.CreateReactionAsync(
                DiscordEmoji.FromName(Program.DiscordClient, ":thumbsup:"));

            // Waits for 30 minutes until to ping people
            ReadOnlyCollection<Reaction>? reactions = await reactionMessage.CollectReactionsAsync(timeBeforeEvent);

            // Gets everyone who reacted to the message
            HashSet<DiscordUser> reactors = new();
            foreach (Reaction? reaction in reactions)
                foreach (DiscordUser? user in reaction.Users)
                    if (!user.IsBot)
                        reactors.Add(user);
            HashSet<ulong> guildReactors = new();
            foreach (DiscordUser reactor in reactors) guildReactors.Add(reactor.Id);

            Console.WriteLine("before adding roles");
            foreach (ulong memberID in guildReactors)
                try
                {
                    DiscordMember? member =
                        await Program.DiscordClient.Guilds[guildEvent.GuildId].GetMemberAsync(memberID);

                    Console.WriteLine(member.Nickname);
                    await member.GrantRoleAsync(targetRole);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
        }

        // Simulates the timeout for reactions
        if (roleCreated) await Task.Delay(timeBeforeEvent);

        builder.Append("Just a heads up, the ");
        builder.Append(guildEvent.Description.ToLower().Contains("match") ? "match " : "scrim ");
        builder.Append("is in ~30 minutes ");
        builder.Append(targetRole.Mention);
        await general.SendMessageAsync(builder.ToString());
        builder.Clear();

        // Give another half hour, will be at start time now
        await Task.Delay(timeUntilEvent);
        builder.Append(targetRole.Mention + " good luck on your ");
        builder.Append(guildEvent.Description.ToLower().Contains("match") ? "match " : "scrim ");
        builder.Append(":3");
        await general.SendMessageAsync(builder.ToString());

        // Wait for 10 minutes after the match starts
        await Task.Delay(timeAfterStart);
        await targetRole.DeleteAsync();
    }
}