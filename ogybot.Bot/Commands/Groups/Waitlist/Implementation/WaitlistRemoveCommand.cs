using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Core.Validators;
using ogybot.Bot.Handlers;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Utility.Extensions;

namespace ogybot.Bot.Commands.Groups.Waitlist.Implementation;

public sealed partial class WaitlistCommands
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("remove", "removes a user from the wait list based on their name or index")]
    public async Task ExecuteRemoveCommandAsync([Summary("users-or-indexes", "The user's name or index")] string usernamesOrIndexes)
    {
        await HandleCommandExecutionAsync(() => RemoveCommandInstructionsAsync(usernamesOrIndexes));
    }

    private async Task RemoveCommandInstructionsAsync(string usernamesOrIndexes)
    {
        if (await IsInvalidContextAsync(ValidChannelId))
        {
            return;
        }

        if (usernamesOrIndexes.Contains(','))
        {
            await RemoveMultiplePlayersFromListAsync(usernamesOrIndexes);
        }
        else
        {
            await RemovePlayerFromListAsync(usernamesOrIndexes);
        }

        await FollowupAsync("Successfully removed provided player from the wait list.");
    }

    private async Task RemoveMultiplePlayersFromListAsync(string usernamesOrIndexes)
    {
        var players = usernamesOrIndexes
            .Split(',')
            .Select(player => player.Trim())
            .Where(player => !player.IsNullOrWhitespace())
            .OrderDescending();

        foreach (var player in players)
        {
            await RemovePlayerFromListAsync(player);
        }
    }

    private async Task RemovePlayerFromListAsync(string usernameOrIndex)
    {

        if (short.TryParse(usernameOrIndex, out var index))
        {
            await RemoveByIndexAsync(index);
        }
        else
        {
            await RemoveByNameAsync(usernameOrIndex);
        }
    }

    private async Task RemoveByIndexAsync(int index)
    {
        var list = await WaitListClient.GetListAsync(WynnGuildId);

        ValidateUserBeingRemovedByIndex(index, list);

        // Gets the user based on the index provided. As the list count starts at 1, the index has to be subtracted by 1.
        var waitListUser = list[index - 1];

        await WaitListClient.RemoveUserAsync(WynnGuildId, waitListUser);
    }

    private async Task RemoveByNameAsync(string username)
    {
        await ValidateUserBeingRemovedByNameAsync(username);

        var waitListUser = new WaitListUser(username);

        await WaitListClient.RemoveUserAsync(WynnGuildId, waitListUser);
    }

    private void ValidateUserBeingRemovedByIndex(int index, IList<WaitListUser> list)
    {
        _commandValidator.ValidateUserRemoval(list, index);
    }

    private async Task ValidateUserBeingRemovedByNameAsync(string username)
    {
        var list = await WaitListClient.GetListAsync(WynnGuildId);

        _commandValidator.ValidateUserRemoval(list, username);
    }
}