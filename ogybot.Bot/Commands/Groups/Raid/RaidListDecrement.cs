using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Core.Validators;
using ogybot.Bot.Handlers;
using ogybot.Domain.Entities;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Utility.Extensions;

namespace ogybot.Bot.Commands.Groups.Raid;

public class RaidListDecrement : BaseRaidCommand
{
    private readonly IListCommandValidator _commandValidator;

    public RaidListDecrement(
        IRaidListClient raidListClient,
        IBotExceptionHandler exceptionHandler,
        IGuildClient guildClient,
        IListCommandValidator commandValidator) : base(raidListClient,
        exceptionHandler,
        guildClient)
    {
        _commandValidator = commandValidator;
    }

    //TODO: make parameters to define whether aspects or emeralds should be decreased and how many.

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("decrement", "Decrements an aspect from the provided user.")]
    public async Task ExecuteCommandAsync([Summary("users-or-indexes", "The user's name or index")] string usernamesOrIndexes)
    {
        await HandleCommandExecutionAsync(() => CommandInstructionsAsync(usernamesOrIndexes));
    }

    private async Task CommandInstructionsAsync(string usernamesOrIndexes)
    {
        if (await IsInvalidContextAsync(ValidChannelId))
        {
            return;
        }

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
        var list = await RaidListClient.GetListAsync(WynnGuildId);

        _commandValidator.ValidateUserRemoval(list, username);

        var aspectListUser = new RaidListUser(username);

        await RaidListClient.DecrementAspectAsync(WynnGuildId, aspectListUser);
    }

    private async Task DecrementByIndexAsync(int index)
    {
        var list = await RaidListClient.GetListAsync(WynnGuildId);

        _commandValidator.ValidateUserRemoval(list, index);

        // Gets the user based on the index provided. As the list count starts at 1, the index has to be subtracted by 1.
        var aspectListUser = list[index - 1];

        await RaidListClient.DecrementAspectAsync(WynnGuildId, aspectListUser);
    }
}