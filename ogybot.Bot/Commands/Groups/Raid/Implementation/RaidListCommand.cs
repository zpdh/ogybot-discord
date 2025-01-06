using Discord;
using Discord.Interactions;
using ogybot.Domain.Entities;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Enums;

namespace ogybot.Bot.Commands.Groups.Raid.Implementation;

public sealed partial class RaidListCommands
{
    private const int DefaultFirstPage = 0;
    private const int DefaultPageSize = 5;
    private static int _currentPage;

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("list", "Presents a list containing information about raid completions per guild member.")]
    public async Task ExecuteListCommandAsync([Summary("order-by")] RaidListOrderType orderType = RaidListOrderType.Raids)
    {
        await HandleCommandExecutionAsync(() => ListCommandInstructionsAsync(orderType));
    }

    private async Task ListCommandInstructionsAsync(RaidListOrderType orderType)
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        _currentPage = DefaultFirstPage;
        var embed = await CreateEmbedAsync(orderType);
        var components = await CreatePaginationComponentsAsync(orderType);

        await FollowupAsync(embed: embed, components: components);
    }

    private async Task<Embed> CreateEmbedAsync(RaidListOrderType orderType)
    {
        var content = await GetEmbedContentAsync(orderType);

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(content.User.Username, content.User.GetAvatarUrl() ?? content.User.GetDefaultAvatarUrl())
            .WithTitle("Raid List")
            .WithDescription(content.Description)
            .WithColor(new Color(211, 63, 30))
            .WithThumbnailUrl("https://wynncraft.wiki.gg/images/8/89/CBRaidIcon.png")
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
        var orderedEnumerable = orderType switch
        {
            RaidListOrderType.Aspects => list.OrderByDescending(user => user.Aspects),
            RaidListOrderType.EmeraldsOwed => list.OrderByDescending(user => user.Emeralds),
            _ => list.OrderByDescending(user => user.Raids)
        };

        // Skips the first x pages of users of the enumerable, then takes the default page size (amount) of users to display.
        return orderedEnumerable.Skip(_currentPage * DefaultPageSize).Take(DefaultPageSize).ToList();
    }

    private static string CreateEmbedDescription(IList<RaidListUser> list)
    {
        var counter = GetInitialCounter();

        return list.Aggregate("", (current, user) => current + FormatUser(counter++, user));
    }

    private static string FormatUser(int index, RaidListUser user)
    {
        return $"**{index}: {user.Username}**\n" +
               $"- {user.Raids} Raids\n" +
               $"- {user.Aspects} Aspects Owed\n" +
               $"- {user.Emeralds} LE Owed\n\n";
    }

    private static int GetInitialCounter()
    {
        return 1 + (_currentPage * DefaultPageSize);
    }

    private async Task<MessageComponent> CreatePaginationComponentsAsync(RaidListOrderType orderType)
    {
        var totalPages = await CalculateTotalPagesAsync();
        var previousButton = CreateButton("\u25c4", $"previous:{orderType}", _currentPage == 0);
        var nextButton = CreateButton("\u25ba", $"next{orderType}", _currentPage >= totalPages - 1);

        return new ComponentBuilder()
            .WithButton(previousButton)
            .WithButton(nextButton)
            .Build();
    }

    private async Task<int> CalculateTotalPagesAsync()
    {
        var list = await RaidListClient.GetListAsync(WynnGuildId);
        return (int)Math.Ceiling((double)list.Count / DefaultPageSize);
    }

    private static ButtonBuilder CreateButton(string label, string customId, bool disabledWhen)
    {
        return new ButtonBuilder()
            .WithLabel(label)
            .WithCustomId(customId)
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(disabledWhen);
    }

    [ComponentInteraction("next:*", true)]
    public async Task HandleNextPageAsync(RaidListOrderType orderType)
    {
        _currentPage++;

        var embed = await CreateEmbedAsync(orderType);
        var components = await CreatePaginationComponentsAsync(orderType);

        await ModifyOriginalMessageAsync(embed, components);
    }

    [ComponentInteraction("previous:*", true)]
    public async Task HandlePreviousPageAsync(RaidListOrderType orderType)
    {
        _currentPage--;

        var embed = await CreateEmbedAsync(orderType);
        var components = await CreatePaginationComponentsAsync(orderType);

        await ModifyOriginalMessageAsync(embed, components);
    }

    private async Task ModifyOriginalMessageAsync(Embed embed, MessageComponent components)
    {
        await ModifyOriginalResponseAsync(msg => {
            msg.Embed = embed;
            msg.Components = components;
        });
    }
}