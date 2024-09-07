using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ogybot.Util;

namespace ogybot.Commands.Other;

/// <summary>
/// Command responsible for pinging chiefs in war-chat
/// </summary>
public class PingChiefsCommand : BaseCommand
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("chiefs", "pings active chiefs")]
    public async Task ExecuteCommandAsync()
    {
        if (await ValidateChannelAsync(GuildChannels.WarChannel)) return;

        await FollowupAsync("<@&1097935496442810419> Wake up", allowedMentions: AllowedMentions.All);
    }
}