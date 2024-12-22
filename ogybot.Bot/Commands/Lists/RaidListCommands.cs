using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Base;
using ogybot.Bot.Commands.Lists.Validators;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Domain.Entities;
using ogybot.Domain.Enums;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Utility.Extensions;

namespace ogybot.Bot.Commands.Lists;

public class RaidListCommands : BasePermissionRequiredCommand
{
    private readonly IRaidListClient _raidListClient;
    private readonly IListCommandValidator _commandValidator;

    private readonly ulong _validChannelId;

    public RaidListCommands(
        IRaidListClient raidListClient,
        IBotExceptionHandler exceptionHandler,
        IListCommandValidator commandValidator,
        IGuildClient guildClient) : base(exceptionHandler, guildClient)
    {
        _raidListClient = raidListClient;
        _commandValidator = commandValidator;
        _validChannelId = ServerConfiguration.RaidsChannel;
    }

    #region List Command

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("raidlist", "Presents the raid list to each player's display raids done, aspects owed and emeralds owed while in the guild.")]
    public async Task ExecuteRaidListCommandAsync([Summary("order-by")] RaidListOrderType orderType = RaidListOrderType.Raids)
    {
        if (await IsInvalidChannelAsync(_validChannelId))
        {
            return;
        }

        await TryExecutingCommandInstructionsAsync(async () => await RaidListCommandInstructionsAsync(orderType));
    }

    private async Task RaidListCommandInstructionsAsync(RaidListOrderType orderType)
    {
        var embed = await CreateEmbedAsync(orderType);

        await FollowupAsync(embed: embed);
    }

    private async Task<Embed> CreateEmbedAsync(RaidListOrderType orderType)
    {
        // Create class to store this info later
        var content = await GetEmbedContentAsync(orderType);

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(content.SocketUser.Username, content.SocketUser.GetAvatarUrl() ?? content.SocketUser.GetDefaultAvatarUrl())
            .WithTitle("Raid List")
            .WithDescription(content.Description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter(content.QueueSize);

        return embedBuilder.Build();
    }

    private async Task<EmbedContent> GetEmbedContentAsync(RaidListOrderType orderType)
    {
        var list = await _raidListClient.GetListAsync();
        var orderedList = CreateOrderedList(list, orderType);

        var user = Context.User;
        var queueSize = "Players in queue: " + list.Count;
        var description = CreateEmbedDescription(orderedList);

        return new EmbedContent(user, queueSize, description);
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

    #endregion

    #region Decrement Command

    // TODO: refactor to include emeralds owed aswell

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("raidlist-decrement", "Decrements an aspect from the provided user.")]
    public async Task ExecuteRaidListDecrementCommandAsync([Summary("users-or-indexes", "The user's name or index")] string usernamesOrIndexes)
    {
        if (await IsInvalidContextAsync(_validChannelId))
        {
            return;
        }

        await TryExecutingCommandInstructionsAsync(async () => await RaidListDecrementCommandInstructionsAsync(usernamesOrIndexes));
    }

    private async Task RaidListDecrementCommandInstructionsAsync(string usernamesOrIndexes)
    {
        if (usernamesOrIndexes.Contains(','))
        {
            await DecrementAspectFromMultiplePlayersAsync(usernamesOrIndexes);
        }
        else
        {
            await DecrementAspectFromPlayerAsync(usernamesOrIndexes);
        }

        await FollowupAsync("Successfully decremented 1 aspect from the provided player(s).");
    }

    private async Task DecrementAspectFromMultiplePlayersAsync(string usernamesOrIndexes)
    {
        var players = usernamesOrIndexes
            .Split(',')
            .Select(player => player.Trim())
            .Where(player => !player.IsNullOrWhitespace())
            .OrderDescending();

        foreach (var player in players)
        {
            await DecrementAspectFromPlayerAsync(player);
        }
    }

    private async Task DecrementAspectFromPlayerAsync(string usernameOrIndex)
    {
        if (short.TryParse(usernameOrIndex, out var index))
        {
            await DecrementByIndexAsync(index);
        }
        else
        {
            await DecrementByNameAsync(usernameOrIndex);
        }
    }

    private async Task DecrementByNameAsync(string username)
    {
        var list = await _raidListClient.GetListAsync();

        _commandValidator.ValidateUserRemoval(list, username);

        var aspectListUser = new RaidListUser(username);

        await _raidListClient.DecrementAspectAsync(aspectListUser);
    }

    private async Task DecrementByIndexAsync(int index)
    {
        var list = await _raidListClient.GetListAsync();

        _commandValidator.ValidateUserRemoval(list, index);

        // Gets the user based on the index provided. As the list count starts at 1, the index has to be subtracted by 1.
        var aspectListUser = list[index - 1];

        await _raidListClient.DecrementAspectAsync(aspectListUser);
    }

    #endregion

}