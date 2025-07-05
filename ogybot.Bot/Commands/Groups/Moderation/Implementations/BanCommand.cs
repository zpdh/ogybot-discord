using Discord;
using Discord.Interactions;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Bot.Commands.Groups.Moderation.Implementation;

public sealed partial class ModerationCommands 
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("bane", "Bans a user.")]
    public async Task ExecuteBanCommandAsync([Summary("discord-uuid", "The discord user id to ban")] string discordUuid)
    {
        await HandleCommandExecutionAsync(() => BanCommandInstructionsAsync(discordUuid));
    }

    private async Task BanCommandInstructionsAsync(string discordUuid)
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

        await UserClient.BanUserAsync(moderatedUser);

        await FollowupAsync("Successfully banned provided user.");
    }
    
}