using Discord;
using Discord.Interactions;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Bot.Commands.Groups.Moderation.Implementation;

public sealed partial class ModerationCommands 
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("ban", "Bans a user.")]
    public async Task ExecuteBanCommandAsync([Summary("discord-uuid", "The discord user id to ban")] ulong discordUuid)
    {
        await HandleCommandExecutionAsync(() => BanCommandInstructionsAsync(discordUuid));
    }

    private async Task BanCommandInstructionsAsync(ulong discordUuid)
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        var moderatedUser = new DiscordUser(discordUuid);

        await UserClient.BanUserAsync(moderatedUser);

        await FollowupAsync("Successfully banned provided user.");
    }
    
}