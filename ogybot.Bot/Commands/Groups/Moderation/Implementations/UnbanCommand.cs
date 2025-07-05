using Discord;
using Discord.Interactions;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Bot.Commands.Groups.Moderation.Implementation;

public sealed partial class ModerationCommands 
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("unban", "Unbans a user.")]
    public async Task ExecuteUnbanCommandAsync([Summary("discord-uuid", "The discord user id to unban")] string discordUuid)
    {
        await HandleCommandExecutionAsync(() => UnbanCommandInstructionsAsync(discordUuid));
    }

    private async Task UnbanCommandInstructionsAsync(string discordUuid)
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

        await UserClient.UnbanUserAsync(moderatedUser);

        await FollowupAsync("Successfully unbanned provided user.");
    }
    
}