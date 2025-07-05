using Discord;
using Discord.Interactions;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Bot.Commands.Groups.Moderation.Implementation;

public sealed partial class ModerationCommands 
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("mute", "Mutes a user.")]
    public async Task ExecuteUnmuteCommandAsync([Summary("discord-uuid", "The discord user id to mute")] string discordUuid)
    {
        await HandleCommandExecutionAsync(() => UnmuteCommandInstructionsAsync(discordUuid));
    }

    private async Task UnmuteCommandInstructionsAsync(string discordUuid)
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        var moderatedUser = new ModeratedUser(discordUuid);

        await UserClient.UnmuteUserAsync(moderatedUser);

        await FollowupAsync("Successfully banned provided user.");
    }
    
}