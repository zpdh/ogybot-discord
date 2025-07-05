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
        if (await IsInvalidContextAsync(ValidChannelId))
        {
            return;
        }

        ulong id = await isValidIdAsync(discordUuid);

        if (id == 0)
        {
            return;
        }
        
        var moderatedUser = new DiscordUser(id);

        await UserClient.MuteUserAsync(moderatedUser);

        await FollowupAsync("Successfully muted provided user.");
    }
    
}