using Discord;
using Discord.Interactions;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Bot.Commands.Groups.Moderation.Implementation;

public sealed partial class ModerationCommands 
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("ban", "Bans a user.")]
    public async Task ExecuteBanCommandAsync([Summary("discord-uuid", "The discord user id to ban")] string discordUuid)
    {
        await HandleCommandExecutionAsync(() => BanCommandInstructionsAsync(discordUuid));
    }

    private async Task BanCommandInstructionsAsync(string discordUuid)
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        var moderatedUser = new ModeratedUser(discordUuid);

        await UserClient.BanUserAsync(moderatedUser);

        await FollowupAsync("Successfully banned provided user.");
    }
    
}