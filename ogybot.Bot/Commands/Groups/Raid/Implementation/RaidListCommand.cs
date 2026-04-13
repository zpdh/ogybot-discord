using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ogybot.Communication.Exceptions;
using ogybot.Domain.Entities;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Enums;
using ogybot.Domain.Primitives;

namespace ogybot.Bot.Commands.Groups.Raid.Implementation;

public sealed partial class RaidListCommands
{
    private const int DefaultFirstPage = 0;
    private const int DefaultPageSize = 5;
    private ulong UserId { get; set; }
    // idea: static page dictionary for each user, which would be reset on each command run
    // somehow make interactions expire
    private static readonly Dictionary<ulong, PageSessionInfo> _sessions = [];
    private static readonly Dictionary<ulong, int> _currentPages = [];

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("list", "Presents a list containing information about raid completions per guild member.")]
    public async Task ExecuteListCommandAsync()
    {
        UserId = Context.User.Id;
        await DeferAsync();
        await HandleCommandExecutionAsync(() => ListCommandInstructionsAsync(RaidListOrderType.Raids));
    }

    private async Task ListCommandInstructionsAsync(RaidListOrderType orderType)
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        _currentPages[UserId] = 0;
        if (_sessions.TryGetValue(UserId, out var session))
        {
            await session.Message.ModifyAsync(msg =>
            {
                msg.Components = new ComponentBuilder().Build();
            });
            session.TimeoutCts.Cancel();
        }

        var embed = await CreateEmbedAsync(orderType);
        var components = new ComponentBuilder().AddRow(await CreateSortComponentAsync(orderType)).AddRow(await CreatePaginationComponentsAsync(orderType)).Build();

        var message = await FollowupAsync(embed: embed, components: components);

        _sessions[Context.User.Id] = new PageSessionInfo
        {
            Message = message,
            TimeoutCts = new(),
        };
        StartOrResetTimeout(UserId, _sessions[UserId]);
    }
    private void StartOrResetTimeout(ulong userId, PageSessionInfo session)
    {
        session.TimeoutCts.Cancel();

        var cts = new CancellationTokenSource();
        session.TimeoutCts = cts;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), cts.Token);

                await session.Message.ModifyAsync(msg =>
                {
                    msg.Components = new ComponentBuilder().Build();
                });

                _sessions.Remove(userId);
                _currentPages.Remove(userId);
            }
            catch (TaskCanceledException)
            {
            }
        });
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

        var orderedList = CreateOrderedList(list, orderType, Context.User.Id);

        var user = Context.User;
        var queueSize = "Players in queue: " + list.Count;
        var description = CreateEmbedDescription(orderedList);

        return EmbedContent.Create(user, queueSize, description);
    }

    private static List<RaidListUser> CreateOrderedList(IList<RaidListUser> list, RaidListOrderType orderType, ulong uid)
    {
        var orderedEnumerable = orderType switch
        {
            RaidListOrderType.Aspects => list.OrderByDescending(user => user.Aspects),
            RaidListOrderType.EmeraldsOwed => list.OrderByDescending(user => user.LiquidEmeralds),
            _ => list.OrderByDescending(user => user.Raids)
        };

        // Skips the first x pages of users of the enumerable, then takes the default page size (amount) of users to display.
        return orderedEnumerable.Skip(_currentPages[uid] * DefaultPageSize).Take(DefaultPageSize).ToList();
    }

    private string CreateEmbedDescription(IList<RaidListUser> list)
    {
        var counter = GetInitialCounter();

        return list.Aggregate("", (current, user) => current + FormatUser(counter++, user));
    }

    private static string FormatUser(int index, RaidListUser user)
    {
        return $"**{index}: {user.McUsername}**\n" +
               $"- {user.Raids} Raids\n" +
               $"- {user.Aspects} Aspects Owed\n" +
               $"- {user.LiquidEmeralds} LE Owed\n\n";
    }

    private int GetInitialCounter()
    {
        return 1 + (_currentPages[UserId] * DefaultPageSize);
    }

    private async Task<ActionRowBuilder> CreateSortComponentAsync(RaidListOrderType orderType)
    {
        return new ActionRowBuilder().WithSelectMenu(CreateSortMenu("Select an option", "order-by", orderType));
    }

    private async Task<ActionRowBuilder> CreatePaginationComponentsAsync(RaidListOrderType orderType)
    {
        var totalPages = await CalculateTotalPagesAsync();
        var previousButton = CreateButton("\u25c4", $"previous:{orderType}", _currentPages[UserId] == 0);
        var nextButton = CreateButton("\u25ba", $"next:{orderType}", _currentPages[UserId] >= totalPages - 1);

        return new ActionRowBuilder()
            .WithButton(previousButton)
            .WithButton(nextButton);
    }

    private async Task<int> CalculateTotalPagesAsync()
    {
        var list = await RaidListClient.GetListAsync(WynnGuildId);
        return (int)Math.Ceiling((double)list.Count / DefaultPageSize);
    }

    private static SelectMenuBuilder CreateSortMenu(string placeholder, string customId, RaidListOrderType orderType)
    {
        return new SelectMenuBuilder()
            .WithPlaceholder(placeholder)
            .WithCustomId(customId)
            .WithMinValues(1)
            .WithMaxValues(1)
            .AddOption("Raids", "0", "Sort by raids completed in descending order.", isDefault: orderType == RaidListOrderType.Raids)
            .AddOption("Aspects", "1", "Sort by aspects owed in descending order.", isDefault: orderType == RaidListOrderType.Aspects)
            .AddOption("Emeralds", "2", "Sort by emeralds owed in descending order", isDefault: orderType == RaidListOrderType.EmeraldsOwed);
    }
    private static ButtonBuilder CreateButton(string label, string customId, bool disabledWhen)
    {
        return new ButtonBuilder()
            .WithLabel(label)
            .WithCustomId(customId)
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(disabledWhen);
    }

    private async Task VerifyPageChange()
    {
        if ((await GetOriginalResponseAsync()).InteractionMetadata.UserId != Context.User.Id) throw new InvalidButton("This is not your command.");
    }

    [ComponentInteraction("next:*", true)]
    public async Task HandleNextPageAsync(RaidListOrderType orderType)
    {
        UserId = Context.User.Id;
        await DeferAsync();
        await HandleCommandExecutionAsync(async () =>
        {
            // TODO: fix static issues and fix ownership issues also make buttons dissapear after 30s
            await VerifyPageChange();
            StartOrResetTimeout(UserId, _sessions[UserId]);
            _currentPages[UserId]++;

            var embed = await CreateEmbedAsync(orderType);
            var components = new ComponentBuilder().AddRow(await CreateSortComponentAsync(orderType)).AddRow(await CreatePaginationComponentsAsync(orderType)).Build();

            await ModifyOriginalMessageAsync(embed, components);
        });
    }

    [ComponentInteraction("previous:*", true)]
    public async Task HandlePreviousPageAsync(RaidListOrderType orderType)
    {
        UserId = Context.User.Id;
        await DeferAsync();
        await HandleCommandExecutionAsync(async () =>
        {
            await VerifyPageChange();
            StartOrResetTimeout(UserId, _sessions[UserId]);
            _currentPages[UserId]--;
            var embed = await CreateEmbedAsync(orderType);
            var components = new ComponentBuilder().AddRow(await CreateSortComponentAsync(orderType)).AddRow(await CreatePaginationComponentsAsync(orderType)).Build();

            await ModifyOriginalMessageAsync(embed, components);
        });
    }

    [ComponentInteraction("order-by", true)]
    public async Task HandleOrderByAsync()
    {
        if (Context.Interaction is SocketMessageComponent component)
        {
            RaidListOrderType orderType = (RaidListOrderType)int.Parse(component.Data.Values.FirstOrDefault("0"));
            UserId = Context.User.Id;
            await DeferAsync();
            await HandleCommandExecutionAsync(async () =>
            {
                await VerifyPageChange();
                StartOrResetTimeout(UserId, _sessions[UserId]);
                _currentPages[UserId] = DefaultFirstPage;
                var embed = await CreateEmbedAsync(orderType);
                var components = new ComponentBuilder().AddRow(await CreateSortComponentAsync(orderType)).AddRow(await CreatePaginationComponentsAsync(orderType)).Build();

                await ModifyOriginalMessageAsync(embed, components);
            });
        }
    }

    private async Task ModifyOriginalMessageAsync(Embed embed, MessageComponent components)
    {
        await ModifyOriginalResponseAsync(msg =>
        {
            msg.Embed = embed;
            msg.Components = components;
        });
    }
}