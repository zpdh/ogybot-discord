using Discord;
using Discord.Interactions;
using ogybot.Domain.Entities;
using ogybot.Domain.Enums;

namespace ogybot.Bot.Commands.Groups.Raid.Implementation;

public sealed partial class RaidListCommands
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("list", "Presents a list containing information about raid completions per guild member.")]
    public async Task ExecuteListCommandAsync([Discord.Interactions.Summary("order-by")] RaidListOrderType orderType = RaidListOrderType.Raids)
    {
        await HandleCommandExecutionAsync(() => ListCommandInstructionsAsync(orderType));
    }

    private async Task ListCommandInstructionsAsync(RaidListOrderType orderType)
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        var embed = await CreateEmbedAsync(orderType);

        await FollowupAsync(embed: embed);
    }

    private async Task<Embed> CreateEmbedAsync(RaidListOrderType orderType)
    {
        // Create class to store this info later
        var content = await GetEmbedContentAsync(orderType);

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(content.User.Username, content.User.GetAvatarUrl() ?? content.User.GetDefaultAvatarUrl())
            .WithTitle("Raid List")
            .WithDescription(content.Description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter(content.QueueSize);

        return embedBuilder.Build();
    }

    private async Task<EmbedContent> GetEmbedContentAsync(RaidListOrderType orderType)
    {
        var list = await RaidListClient.GetListAsync(WynnGuildId);
        var orderedList = CreateOrderedList(list, orderType);

        var user = Context.User;
        var queueSize = "Players in queue: " + list.Count;
        var description = CreateEmbedDescription(orderedList);

        return EmbedContent.Create(user, queueSize, description);
    }

    private static List<RaidListUser> CreateOrderedList(IList<RaidListUser> list, RaidListOrderType orderType)
    {
        return orderType switch
        {
            RaidListOrderType.Aspects => list.OrderByDescending(user => user.Aspects).ToList(),
            RaidListOrderType.EmeraldsOwed => list.OrderByDescending(user => user.EmeraldsOwed).ToList(),
            _ => list.OrderByDescending(user => user.Raids).ToList()
        };
    }

    private static string CreateEmbedDescription(IList<RaidListUser> list)
    {
        var description = "";

        var counter = 1;

        foreach (var raidListUser in list)
        {
            description +=
                $"{counter}. {raidListUser.Username}: {raidListUser.Raids} Raids | {raidListUser.Aspects} Aspects Owed | {raidListUser.EmeraldsOwed} Emeralds Owed\n\n";

            counter++;
        }

        return description;
    }
}