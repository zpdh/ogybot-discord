using Discord;
using Discord.Interactions;
using ogybot.Domain.Entities;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Bot.Commands.Groups.Waitlist.Implementation;

public sealed partial class WaitlistCommands
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("list", "Shows the list to rejoin the guild.")]
    public async Task ExecuteListCommandAsync()
    {
        await HandleCommandExecutionAsync(ListCommandInstructionsAsync);
    }

    private async Task ListCommandInstructionsAsync()
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        var embed = await CreateEmbedAsync();

        await FollowupAsync(embed: embed);
    }

    private async Task<Embed> CreateEmbedAsync()
    {
        // Create class to store this info later
        var content = await GetEmbedContentAsync();

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(content.User.Username, content.User.GetAvatarUrl() ?? content.User.GetDefaultAvatarUrl())
            .WithTitle("Waitlist")
            .WithDescription(content.Description)
            .WithColor(new Color(249, 252, 154))
            .WithThumbnailUrl("https://wynncraft.wiki.gg/images/0/04/ShinyIcon.png")
            .WithCurrentTimestamp()
            .WithFooter(content.QueueSize);

        return embedBuilder.Build();
    }

    private async Task<EmbedContent> GetEmbedContentAsync()
    {
        var list = await WaitListClient.GetListAsync(WynnGuildId);

        var user = Context.User;
        var queueSize = "Players in queue: " + list.Count;
        var description = CreateEmbedDescription(list);

        return EmbedContent.Create(user, queueSize, description);
    }

    private static string CreateEmbedDescription(IList<WaitListUser> list)
    {
        var counter = 1;

        return list.Aggregate("", (current, waitlistUser) => current + $"**{counter++}.** {waitlistUser.Username}\n");
    }
}