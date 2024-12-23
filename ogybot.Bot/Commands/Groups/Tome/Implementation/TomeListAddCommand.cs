using Discord;
using Discord.Interactions;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Bot.Commands.Groups.Tome.Implementation;

public sealed partial class TomeListCommands
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("add", "Adds a user to the tome list.")]
    public async Task ExecuteAddCommandAsync([Summary("user", "User to be added into the tome list")] string username)
    {
        await HandleCommandExecutionAsync(() => RemoveCommandInstructionsAsync(username));
    }

    private async Task AddCommandInstructionsAsync(string username)
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        await ValidateUsernameAsync(username);

        await AddUserToTomeListAsync(username);

        await FollowupAsync($"Successfully added player {username} to the tome list.");
    }

    private async Task AddUserToTomeListAsync(string username)
    {
        var tomeListUser = new TomeListUser(username);

        await TomeListClient.AddUserAsync(WynnGuildId, tomeListUser);
    }

    private async Task ValidateUsernameAsync(string username)
    {
        var userList = await TomeListClient.GetListAsync(WynnGuildId);
        _commandValidator.ValidateUsername(userList, username);
    }
}