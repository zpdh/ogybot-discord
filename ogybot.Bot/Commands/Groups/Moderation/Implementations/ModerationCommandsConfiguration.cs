using ogybot.Bot.Commands.Core.Validators;
using ogybot.Bot.Commands.Groups.Raid;
using ogybot.Bot.Handlers;
using ogybot.Domain.Accessors;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Moderation.Implementation;

public sealed partial class ModerationCommands : ModerationCommand
{
    public ModerationCommands(
        IBotExceptionHandler exceptionHandler,
        IServerConfigurationAccessor configurationAccessor,
        IUserClient userClient) : base(exceptionHandler, configurationAccessor, userClient)
    {
    }
}