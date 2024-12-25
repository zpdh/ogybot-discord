using Discord.Interactions;
using ogybot.Bot.Commands.Core.Base;
using ogybot.Bot.Handlers;
using ogybot.CrossCutting.Accessors.Abstractions;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Waitlist;

[Group("waitlist", "Presents a collection of waitlist related commands.")]
public abstract class BaseWaitlistCommand : BasePermissionRequiredCommand
{
    protected readonly IWaitListClient WaitListClient;

    protected ulong ValidChannelId { get; set; }
    protected Guid WynnGuildId { get; set; }

    protected BaseWaitlistCommand(
        IBotExceptionHandler exceptionHandler,
        IServerConfigurationAccessor configurationAccessor,
        IWaitListClient waitListClient) : base(exceptionHandler, configurationAccessor)
    {
        WaitListClient = waitListClient;
    }

    protected override void ConfigureCommandSettings()
    {
        ValidChannelId = ServerConfiguration.LayoffsChannel;
        WynnGuildId = ServerConfiguration.WynnGuildId;
    }
}