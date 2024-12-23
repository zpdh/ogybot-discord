using Discord;
using Discord.Interactions;
using ogybot.Bot.Handlers;
using ogybot.Domain.Entities;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Tome.Implementation;

public sealed partial class TomeListCommands
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("list", "Shows the current queue to get a guild tome.")]
    public async Task ExecuteListCommandAsync()
    {
        await HandleCommandExecutionAsync(ListCommandInstructionsAsync, false);
    }

    private async Task ListCommandInstructionsAsync()
    {
        var embed = await CreateEmbedAsync();

        await FollowupAsync(embed: embed);
    }

    private async Task<Embed> CreateEmbedAsync()
    {
        var content = await GetEmbedContentAsync();

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(content.User.Username, content.User.GetAvatarUrl() ?? content.User.GetDefaultAvatarUrl())
            .WithTitle("Tome List")
            .WithDescription(content.Description)
            .WithColor(new Color(0,187,189))
            .WithThumbnailUrl("https://wynncraft.wiki.gg/images/8/8f/CBWorldEventIcon.png")
            .WithCurrentTimestamp()
            .WithFooter(content.QueueSize);

        return embedBuilder.Build();
    }

    private async Task<EmbedContent> GetEmbedContentAsync()
    {
        var list = new List<TomeListUser>();

        var user = Context.User;
        var queueSize = "Players in queue: " + list.Count;
        var description = CreateEmbedDescription(list);

        return EmbedContent.Create(user, queueSize, description);
    }

    private static string CreateEmbedDescription(IList<TomeListUser> list)
    {
        var description = "";

        var counter = 1;

        foreach (var tomeListUser in list)
        {
            description += $"{counter}. {tomeListUser.Username}\n";

            counter++;
        }

        return description;
    }
}