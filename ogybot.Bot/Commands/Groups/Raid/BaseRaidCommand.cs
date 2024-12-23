﻿using Discord.Interactions;
using ogybot.Bot.Commands.Core.Base;
using ogybot.Bot.Handlers;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Raid;

[Group("raid-list", "Collection of commands regarding raids lists.")]
public abstract class BaseRaidCommand : BasePermissionRequiredCommand
{
    protected readonly IRaidListClient RaidListClient;

    protected ulong ValidChannelId { get; set; }
    protected Guid WynnGuildId { get; set; }

    protected BaseRaidCommand(
        IRaidListClient raidListClient,
        IBotExceptionHandler exceptionHandler,
        IGuildClient guildClient) : base(exceptionHandler, guildClient)
    {
        RaidListClient = raidListClient;
    }

    protected override void ConfigureCommandSettings()
    {
        ValidChannelId = ServerConfiguration.RaidsChannel;
        WynnGuildId = ServerConfiguration.WynnGuildId;
    }
}