using Discord;
using Discord.Interactions;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Utility.Extensions;

namespace ogybot.Bot.Commands.Groups.Tome.Implementation;

public sealed partial class TomeListCommands
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("remove", "removes a user from the tome list based on their name or index")]
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

        await FollowupAsync("Successfully removed provided player(s) from the wait list.");
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
        var list = await TomeListClient.GetListAsync(WynnGuildId);

        ValidateUserBeingRemovedByIndex(index, list);

        // Gets the user based on the index provided. As the list count starts at 1, the index has to be subtracted by 1.
        var tomeListUser = list[index - 1];

        await TomeListClient.RemoveUserAsync(WynnGuildId, tomeListUser);
    }

    private async Task RemoveByNameAsync(string username)
    {
        await ValidateUserBeingRemovedByNameAsync(username);

        var tomeListUser = new TomeListUser(username);

        await TomeListClient.RemoveUserAsync(WynnGuildId, tomeListUser);
    }

    private async Task ValidateUserBeingRemovedByNameAsync(string username)
    {
        var list = await TomeListClient.GetListAsync(WynnGuildId);

        _commandValidator.ValidateUserRemoval(list, username);
    }

    private void ValidateUserBeingRemovedByIndex(int index, IList<TomeListUser> list)
    {
        _commandValidator.ValidateUserRemoval(list, index);
    }
}