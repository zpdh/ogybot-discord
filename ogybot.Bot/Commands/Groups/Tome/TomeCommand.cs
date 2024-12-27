using Discord.Interactions;
using ogybot.Bot.Commands.Core.Base;
using ogybot.Bot.Handlers;
using ogybot.Domain.Accessors;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Tome;

[Group("tome-list", "Presents a collection of tome list related commands.")]
public abstract class TomeCommand : PermissionRequiredCommand
{
    protected readonly ITomeListClient TomeListClient;

    protected ulong ValidChannelId { get; set; }
    protected Guid WynnGuildId { get; set; }

    protected TomeCommand(
        IBotExceptionHandler exceptionHandler,
        IServerConfigurationAccessor configurationAccessor,
        ITomeListClient tomeListClient) : base(exceptionHandler, configurationAccessor)
    {
        TomeListClient = tomeListClient;
    }

    protected override void ConfigureCommandSettings()
    {
        ValidChannelId = ServerConfiguration.TomeChannel;
        WynnGuildId = ServerConfiguration.WynnGuildId;
    }
}