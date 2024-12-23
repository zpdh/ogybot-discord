using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Core.Validators;
using ogybot.Bot.Handlers;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Waitlist;

public sealed class WaitlistAddCommand : BaseWaitlistCommand
{
    private readonly IListCommandValidator _commandValidator;

    public WaitlistAddCommand(
        IBotExceptionHandler exceptionHandler,
        IGuildClient guildClient,
        IWaitListClient waitListClient,
        IListCommandValidator commandValidator) : base(
        exceptionHandler,
        guildClient,
        waitListClient)
    {
        _commandValidator = commandValidator;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("add", "Adds a user to the wait list.")]
    public async Task ExecuteCommandAsync([Summary("user", "User to insert into the wait list")] string username)
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
        _commandValidator.ValidateUsername(userList, username);
    }

}