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
        if (await ValidateChannelAsync(GuildChannels.WarChannel)) return;

        var guildMessage = guildAttacking is not null
            ? $"\n**Guild:** {guildAttacking}"
            : "";

        //Heavy raid
        if (raidType.ToString() == "0")
        {
            await FollowupAsync($"<@&1044407292340211793>{guildMessage}",
                allowedMentions: AllowedMentions.All);
        }
        //Light raid
        else
        {
            await FollowupAsync($"<@&1044407413345886238>{guildMessage}",
                allowedMentions: AllowedMentions.All);
        }
    }
}