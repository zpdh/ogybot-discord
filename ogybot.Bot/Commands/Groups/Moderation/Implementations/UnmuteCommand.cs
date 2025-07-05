using Discord;
using Discord.Interactions;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Bot.Commands.Groups.Moderation.Implementation;

public sealed partial class ModerationCommands 
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("unmute", "Mutes a user.")]
    public async Task ExecuteUnmuteCommandAsync([Summary("discord-uuid", "The discord user id to unmute")] string discordUuid)
    {
        await HandleCommandExecutionAsync(() => UnmuteCommandInstructionsAsync(discordUuid));
    }

    private async Task UnmuteCommandInstructionsAsync(string discordUuid)
    {
        if (await IsInvalidContextAsync(ValidChannelId))
        {
            return;
        }

        var id = await isValidIdAsync(discordUuid);

        if (id == 0)
        {
            return;
        }
        
        var moderatedUser = new DiscordUser(id);

        await UserClient.UnmuteUserAsync(moderatedUser);

        await FollowupAsync("Successfully unmuted provided user.");
    }
    
}