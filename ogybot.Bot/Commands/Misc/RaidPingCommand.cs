using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Base;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Communication.Exceptions;
using ogybot.Domain.Enums;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Utility.Extensions;

namespace ogybot.Bot.Commands.Misc;

public class RaidPingCommand : BaseCommand
{

    public RaidPingCommand(IBotExceptionHandler exceptionHandler, IGuildClient guildClient) : base(exceptionHandler, guildClient)
    {
    }

    private ulong ValidChannelId { get; set; }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("raid", "Pings the provided raid role (Heavy/Light Raid)")]
    public async Task ExecuteCommandAsync(RaidType raidType, [Summary("guild", "The guild attacking our claim")] string? guildAttacking = null)
    {
        var serverConfig = await GetServerConfigurationAsync();
        ValidChannelId = serverConfig.RaidsChannel;

        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        var raidRoleId = DetermineRaidRole(raidType);
        var guildMessage = CreateGuildMessage(raidRoleId, guildAttacking);

        await FollowupAsync(guildMessage, allowedMentions: AllowedMentions.All);
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

    private static ulong DetermineRaidRole(RaidType raidType)
    {
        return raidType switch
        {
            RaidType.LightRaid => GuildRoleIds.LightRaidRole,

            RaidType.HeavyRaid => GuildRoleIds.HeavyRaidRole,

            _ => throw new InvalidCommandArgumentException(ErrorMessages.InvalidRaidTypeError)
        };
    }
}