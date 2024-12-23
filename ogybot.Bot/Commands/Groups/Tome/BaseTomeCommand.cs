using Discord.Interactions;
using ogybot.Bot.Commands.Core.Base;
using ogybot.Bot.Handlers;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Tome;

[Group("tome-list", "Collection of commands regarding tome list.")]
public class BaseTomeCommand : BasePermissionRequiredCommand
{
    protected readonly ITomeListClient TomeListClient;

    protected ulong ValidChannelId { get; set; }
    protected Guid WynnGuildId { get; set; }

    public BaseTomeCommand(
        IBotExceptionHandler exceptionHandler,
        IGuildClient guildClient,
        ITomeListClient tomeListClient) : base(exceptionHandler, guildClient)
    {
        TomeListClient = tomeListClient;
    }

    protected override void ConfigureCommandSettings()
    {
        ValidChannelId = ServerConfiguration.TomeChannel;
        WynnGuildId = ServerConfiguration.WynnGuildId;
    }
}