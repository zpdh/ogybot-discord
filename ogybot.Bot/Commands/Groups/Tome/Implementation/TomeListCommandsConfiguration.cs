using ogybot.Bot.Commands.Core.Validators;
using ogybot.Bot.Handlers;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Tome.Implementation;

public sealed partial class TomeListCommands : BaseTomeCommand
{
    private readonly IListCommandValidator _commandValidator;

    public TomeListCommands(
        IBotExceptionHandler exceptionHandler,
        IGuildClient guildClient,
        ITomeListClient tomeListClient,
        IListCommandValidator commandValidator) : base(exceptionHandler, guildClient, tomeListClient)
    {
        _commandValidator = commandValidator;
    }
}