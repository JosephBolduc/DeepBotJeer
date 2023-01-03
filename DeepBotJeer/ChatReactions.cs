using System.Collections;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace SpaceBallsBot;

public static class ChatReactions
{
    private static readonly Random Rng = new();

    private static readonly ulong[] TargetedUsers =
    {
        294937952314851329, // addison
        241958250931683329, // hank
        254661838497644544, // ben
        300507901892624385 // me
    };

    private static readonly string[] BombWords =
    {
        "bomb",
        "explosive",
        "explode",
        "c4",
        "payload cart",
        "semtex",
        "tatp",
        "urea nitrate",
        "triacetone triperoxide",
        "tannerite",
        "academy sports and outdoors",
        "thermite"
    };

    private static readonly ArrayList UsedGifIndexes = new();

    public static async void Handler(DiscordClient s, MessageCreateEventArgs e)
    {
        if (BombWords.Any(e.Message.Content.ToLower().Contains))
        {
            var response = CreateThreat();
            await e.Channel.SendMessageAsync(response);
        }

        if (e.Message.Content.ToLower().Contains("zaza"))
            await e.Message.RespondAsync("when you outside and smell that zaza breh");

        if (!TargetedUsers.Contains(e.Author.Id)) return;

        if (Rng.Next(30) == 0)
        {
            var response = RandomReactionGif();
            await e.Channel.SendMessageAsync(response);
        }
    }

    private static string CreateThreat()
    {
        string[] maps =
        {
            "pl_badwater",
            "pl_barnblitz",
            "pl_borneo",
            "pl_swiftwater",
            "pl_upward",
            "pl_thundermountain",
            "pl_snowycoast",
            "plr_bananabay",
            "plr_hightower"
        };
        string[] cart =
        {
            "payload",
            "bomb",
            "cart"
        };

        // Creating the sentence, starting at normal english
        var response = "";
        if (Rng.Next(3) == 0) response += "Ok so imagine ";
        response += maps[Rng.Next(maps.Length)];
        response += " but instead I push the ";
        response += cart[Rng.Next(cart.Length)];
        response += " into your house.";

        // Lowercase + no period modifier
        if (Rng.Next(2) == 0)
        {
            response = response.ToLower();
            response = response[..^1];
        }

        // Yelling modifier
        if (Rng.Next(25) == 0)
        {
            response = response.ToUpper();
            response = response[..^1]; // Strips last character, somehow...
            response += Exclam();
        }

        return response;
    }

    private static string RandomReactionGif()
    {
        string[] gifUrls =
        {
            "https://media.discordapp.net/attachments/528793773346652160/1000667563715481600/F054CF2C-9261-4662-898F-4481D9E65662.gif",
            "https://media.discordapp.net/attachments/992510545997615145/1042905544828133527/popoki.gif",
            "https://media.discordapp.net/attachments/951154860412268624/1038958172796375060/attachment-1.gif",
            "https://media.discordapp.net/attachments/970524905634398258/1043377088343855114/image0_36508163147553.gif",
            "https://media.discordapp.net/attachments/970524905634398258/1043376755764891658/asdafa.gif",
            "https://media.discordapp.net/attachments/970524905634398258/1043379101274226698/C7724FD9-B527-4026-A5D1-A2B13D9753CD.gif",
            "https://media.discordapp.net/attachments/649814265347309578/1020017397454864404/funnt.gif",
            "https://media.discordapp.net/attachments/806714809462161478/1020093536798003220/bimplyactvity.gif",
            "https://media.discordapp.net/attachments/697671104046825533/977753307198148608/14D3DE03-80F9-4823-902A-73848B83645A.gif",
            "https://media.discordapp.net/attachments/970512579057287234/1031794357432487956/4521A477-1804-4CD2-874D-9C826DDD1A31.gif",
            "https://tenor.com/view/jerma-psycho-streamer-jerma985-jerma-sus-gif-25065540",
            "https://media.discordapp.net/attachments/813078853399478282/959886454669062214/funny.gif",
            "https://media.discordapp.net/attachments/838736235437883412/998292828910272612/94BBE4D0-73DD-4B50-868E-66C44B0464CD.gif",
            "https://media.discordapp.net/attachments/970512579321536523/1022027361161580564/4C93B042-327F-428E-BDF1-23E2C78C95DF.gif",
            "https://media.discordapp.net/attachments/811646862729150555/937444902676099122/dserver2-1.gif"
        };

        int gifUrlIndex;
        while (true)
        {
            gifUrlIndex = Rng.Next(gifUrls.Length);
            if (!UsedGifIndexes.Contains(gifUrlIndex)) break;
        }

        UsedGifIndexes.Add(gifUrlIndex);
        if (gifUrls.Length == UsedGifIndexes.Count) UsedGifIndexes.Clear();
        return gifUrls[gifUrlIndex];
    }

    private static string Exclam()
    {
        var length = Rng.Next(4, 15);
        var response = "";

        for (var i = 0; i < length; i++)
            if (Rng.Next(i) == 0) response += "!";
            else response += "1";

        return response;
    }
}