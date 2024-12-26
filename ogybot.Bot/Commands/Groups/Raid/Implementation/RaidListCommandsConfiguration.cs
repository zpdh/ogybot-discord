using ogybot.Bot.Commands.Core.Validators;
using ogybot.Bot.Handlers;
using ogybot.Domain.Accessors;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Raid.Implementation;

public sealed partial class RaidListCommands : BaseRaidCommand
{
    private readonly IListCommandValidator _commandValidator;

    public RaidListCommands(
        IRaidListClient raidListClient,
        IBotExceptionHandler exceptionHandler,
        IServerConfigurationAccessor configurationAccessor,
        IListCommandValidator commandValidator) : base(raidListClient, exceptionHandler, configurationAccessor)
    {
        _commandValidator = commandValidator;
    }
}