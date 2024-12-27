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
        await HandleCommandExecutionAsync(() => AddCommandInstructionsAsync(username));
    }

    private async Task AddCommandInstructionsAsync(string username)
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        _commandValidator.ValidateUsername(username);

        await AddUserToTomeListAsync(username);

        await FollowupAsync($"Successfully added player {username} to the tome list.");
    }

    private async Task AddUserToTomeListAsync(string username)
    {
        var tomeListUser = new TomeListUser(username);

        await TomeListClient.AddUserAsync(WynnGuildId, tomeListUser);
    }
}