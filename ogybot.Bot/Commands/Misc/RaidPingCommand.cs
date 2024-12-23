﻿using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Core.Base;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Communication.Exceptions;
using ogybot.Domain.Enums;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Utility.Extensions;

namespace ogybot.Bot.Commands.Misc;

public sealed class RaidPingCommand : BaseCommand
{
    private ulong ValidChannelId { get; set; }

    public RaidPingCommand(IBotExceptionHandler exceptionHandler, IGuildClient guildClient) : base(exceptionHandler, guildClient)
    {
    }

    protected override void ConfigureCommandSettings()
    {
        ValidChannelId = ServerConfiguration.WarChannel;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("raid", "Pings the provided raid role (Heavy/Light Raid)")]
    public async Task ExecuteCommandAsync(RaidType raidType, [Summary("guild", "The guild attacking our claim")] string? guildAttacking = null)
    {
        await HandleCommandExecutionAsync(() => CommandInstructionsAsync(raidType, guildAttacking));
    }

    private async Task CommandInstructionsAsync(RaidType raidType, string? guildAttacking)
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        var raidRoleId = DetermineRaidRole(raidType);
        var guildMessage = CreateGuildMessage(raidRoleId, guildAttacking);

        await FollowupAsync(guildMessage, allowedMentions: AllowedMentions.All);
    }

    private static ulong DetermineRaidRole(RaidType raidType)
    {
        return raidType switch
        {
            RaidType.LightRaid => GuildRoleIds.LightRaidRole,

            RaidType.HeavyRaid => GuildRoleIds.HeavyRaidRole,

            _ => throw new InvalidCommandArgumentException(ErrorMessages.InvalidRaidTypeError)
        };
    }

    private static string CreateGuildMessage(ulong raidRoleId, string? guildAttacking)
    {
        var guildMessage = $"<@&{raidRoleId}>";

        if (!guildAttacking.IsNullOrWhitespace())
        {
            guildMessage += $"\n**Guild**: {guildAttacking}";
        }

        return guildMessage;
    }
}