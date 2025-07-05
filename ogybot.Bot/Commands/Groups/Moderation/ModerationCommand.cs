using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Core.Base;
using ogybot.Bot.Handlers;
using ogybot.Domain.Accessors;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Moderation;

[Group("moderation", "Pressents a collection of moderation related commands")]
public abstract class  ModerationCommand : HighPermissionRequiredCommand
{
    protected readonly IUserClient UserClient;

    protected ulong ValidChannelId { get; set; }
    protected ModerationCommand(
        IBotExceptionHandler exceptionHandler,
        IServerConfigurationAccessor configurationAccessor,
        IUserClient userClient) : base(exceptionHandler, configurationAccessor)
    {
        UserClient = userClient;
    }

    protected override void ConfigureCommandSettings()
    {
        ValidChannelId = ServerConfiguration.BroadcastingChannel;
    }
}