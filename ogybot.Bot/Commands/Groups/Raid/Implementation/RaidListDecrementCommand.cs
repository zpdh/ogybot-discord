using Discord;
using Discord.Interactions;
using ogybot.Communication.Exceptions;
using ogybot.Domain.DataTransferObjects;
using ogybot.Utility.Extensions;

namespace ogybot.Bot.Commands.Groups.Raid.Implementation;

public sealed partial class RaidListCommands
{
    //TODO: make parameters to define whether aspects or emeralds should be decreased and how many.

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("decrement", "Decrements an aspect from the provided user.")]
    public async Task ExecuteDecrementCommandAsync(
        [Summary("users-or-indexes", "The user's name or index")]
        string usernamesOrIndexes,
        [Summary("aspects", "Number of aspects to remove from the provided user")]
        double aspectAmount = 0,
        [Summary("LE", "Number of liquid emeralds to remove from the provided user")]
        double liquidEmeraldAmount = 0)
    {
        await HandleCommandExecutionAsync(() => DecrementCommandInstructionsAsync(usernamesOrIndexes, aspectAmount, liquidEmeraldAmount));
    }

    private async Task DecrementCommandInstructionsAsync(string usernamesOrIndexes, double aspectAmount, double liquidEmeraldAmount)
    {
        if (await IsInvalidContextAsync(ValidChannelId))
        {
            return;
        }

        if (usernamesOrIndexes.Contains(','))
        {
            await DecrementAspectFromMultiplePlayersAsync(usernamesOrIndexes, aspectAmount, liquidEmeraldAmount);
        }
        else
        {
            await DecrementAspectFromPlayerAsync(usernamesOrIndexes, aspectAmount, liquidEmeraldAmount);
        }

        await FollowupAsync("Successfully decremented rewards from the valid provided player(s).");
    }

    private async Task DecrementAspectFromMultiplePlayersAsync(string usernamesOrIndexes, double aspectAmount, double liquidEmeraldAmount)
    {
        var players = usernamesOrIndexes
            .Split(',')
            .Select(player => player.Trim())
            .Where(player => !player.IsNullOrWhitespace())
            .OrderDescending()
            .ToArray();

        // TEMPORARY CODE
        var tasks = players.Select(async (player) => {
            try
            {
                await DecrementAspectFromPlayerAsync(player, aspectAmount, liquidEmeraldAmount);
            }
            catch (Exception)
            {
                return player;
            }

            return null;
        }).ToArray();

        var results = await Task.WhenAll(tasks);
        var errors = results.Where(result => result != null).ToArray();
        var errorMsg = errors.Aggregate("Could not remove the following players:", (current, err) => current + $" {err}");

        if (errors.Length != 0)
        {
            throw new InvalidCommandArgumentException(errorMsg);
        }

        // END OF TEMPORARY CODE
    }

    private async Task DecrementAspectFromPlayerAsync(string usernameOrIndex, double aspectAmount, double liquidEmeraldAmount)
    {
        if (ushort.TryParse(usernameOrIndex, out var index))
        {
            await DecrementByIndexAsync(index, aspectAmount, liquidEmeraldAmount);
        }
        else
        {
            await DecrementByNameAsync(usernameOrIndex, aspectAmount, liquidEmeraldAmount);
        }
    }

    private async Task DecrementByNameAsync(string username, double aspectAmount, double liquidEmeraldAmount)
    {
        var list = await RaidListClient.GetListAsync(WynnGuildId);

        _commandValidator.ValidateUserRemoval(list, username);

        var dto = new RaidListUserDto(username, aspectAmount, liquidEmeraldAmount);

        await RaidListClient.DecrementRewardsAsync(WynnGuildId, dto);
    }

    private async Task DecrementByIndexAsync(int index, double aspectAmount, double liquidEmeraldAmount)
    {
        var list = await RaidListClient.GetListAsync(WynnGuildId);

        _commandValidator.ValidateUserRemoval(list, index);

        // Gets the user based on the index provided. As the list count starts at 1, the index has to be subtracted by 1.
        var aspectListUser = list[index - 1];

        var dto = new RaidListUserDto(aspectListUser.Username, aspectAmount, liquidEmeraldAmount);

        await RaidListClient.DecrementRewardsAsync(WynnGuildId, dto);
    }
}