using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Core.Validators;
using ogybot.Bot.Handlers;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Tome;

public class TomeListAddCommand : BaseTomeCommand
{
    private readonly IListCommandValidator _commandValidator;

    public TomeListAddCommand(
        IBotExceptionHandler exceptionHandler,
        IGuildClient guildClient,
        ITomeListClient tomeListClient,
        IListCommandValidator commandValidator) : base(exceptionHandler, guildClient, tomeListClient)
    {
        _commandValidator = commandValidator;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("add", "Adds a user to the tome list.")]
    public async Task ExecuteCommandAsync([Summary("user", "User to be added into the tome list")] string username)
    {
        await HandleCommandExecutionAsync(() => CommandInstructionsAsync(username));
    }

    private async Task CommandInstructionsAsync(string username)
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