﻿using Discord.Interactions;
using ogybot.Bot.Commands.Core.Base;
using ogybot.Bot.Commands.Core.Validators;
using ogybot.Bot.Handlers;
using ogybot.Domain.Accessors;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Raid;

[Group("raid-list", "Presents a collection of raid list related commands.")]
public abstract class BaseRaidCommand : BasePermissionRequiredCommand
{
    protected readonly IRaidListClient RaidListClient;

    protected ulong ValidChannelId { get; set; }
    protected Guid WynnGuildId { get; set; }

    protected BaseRaidCommand(
        IRaidListClient raidListClient,
        IBotExceptionHandler exceptionHandler,
        IServerConfigurationAccessor configurationAccessor) : base(exceptionHandler, configurationAccessor)
    {
        RaidListClient = raidListClient;
    }

    protected override void ConfigureCommandSettings()
    {
        ValidChannelId = ServerConfiguration.RaidsChannel;
        WynnGuildId = ServerConfiguration.WynnGuildId;
    }
}