using Discord;
using Discord.Interactions;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Bot.Commands.Groups.Moderation.Implementation;

public sealed partial class ModerationCommands 
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("mute", "Mutes a user.")]
    public async Task ExecuteMuteCommandAsync([Summary("discord-uuid", "The discord user id to mute")] string discordUuid)
    {
        await HandleCommandExecutionAsync(() => MuteCommandInstructionsAsync(discordUuid));
    }

    private async Task MuteCommandInstructionsAsync(string discordUuid)
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        var moderatedUser = new DiscordUser(discordUuid);

        await UserClient.MuteUserAsync(moderatedUser);

        await FollowupAsync("Successfully banned provided user.");
    }
    
}