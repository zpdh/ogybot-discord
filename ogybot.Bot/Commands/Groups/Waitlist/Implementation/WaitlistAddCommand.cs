using Discord;
using Discord.Interactions;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Bot.Commands.Groups.Waitlist.Implementation;

public sealed partial class WaitlistCommands
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("add", "Adds a user to the wait list.")]
    public async Task ExecuteAddCommandAsync([Summary("user", "User to insert into the wait list")] string username)
    {
        await HandleCommandExecutionAsync(() => AddCommandInstructionsAsync(username));
    }

    private async Task AddCommandInstructionsAsync(string username)
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        await ValidateUsernameAsync(username);

        await AddUserToWaitlistAsync(username);

        await FollowupAsync($"Successfully added player {username} to the wait list.");
    }

    private async Task AddUserToWaitlistAsync(string username)
    {
        var waitListUser = new WaitListUser(username);

        await WaitListClient.AddUserAsync(WynnGuildId, waitListUser);
    }

    private async Task ValidateUsernameAsync(string username)
    {
        var userList = await WaitListClient.GetListAsync(WynnGuildId);
        _commandValidator.ValidateUsername(username);
    }
}