using ogybot.Bot.Commands.Core.Validators;
using ogybot.Bot.Handlers;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Waitlist.Implementation;

public sealed partial class WaitlistCommands : BaseWaitlistCommand
{
    private readonly IListCommandValidator _commandValidator;

    public WaitlistCommands(
        IBotExceptionHandler exceptionHandler,
        IGuildClient guildClient,
        IWaitListClient waitListClient,
        IListCommandValidator commandValidator) : base(exceptionHandler, guildClient, waitListClient)
    {
        _commandValidator = commandValidator;
    }
}