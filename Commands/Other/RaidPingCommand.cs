using Discord;
using Discord.Interactions;
using ogybot.Util;

namespace ogybot.Commands.Other;

public enum RaidType
{
    LightRaid = 0,
    HeavyRaid = 1
}

/// <summary>
/// Command responsible for pinging Raid roles.
/// </summary>
public class RaidPingCommand : BaseCommand
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("raid", "pings heavy/light raid")]
    public async Task ExecuteCommandAsync(
        RaidType raidType,
        [Summary("guild", "the guild attacking us")]
        string? guildAttacking = null)
    {
        if (await IsInvalidChannelAsync(GuildChannels.WarChannel)) return;

        var guildMessage = guildAttacking is not null
            ? $"\n**Guild:** {guildAttacking}"
            : string.Empty;

        //Heavy raid
        if (raidType.ToString() == "0")
        {
            await FollowupAsync($"<@&{GuildPings.HeavyRaidRole}>{guildMessage}",
                allowedMentions: AllowedMentions.All);
        }
        //Light raid
        else
        {
            await FollowupAsync($"<@&{GuildPings.LightRaidRole}>{guildMessage}",
                allowedMentions: AllowedMentions.All);
        }
    }
}