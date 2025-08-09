using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Core.Base;
using ogybot.Bot.Handlers;
using ogybot.Data.Clients;
using ogybot.Domain.Accessors;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Misc.Moderation;

public sealed class BanCommand : HighPermissionRequiredCommand
{
    private ulong ValidChannelId { get; set; }
    private readonly IUserClient UserClient;

    public BanCommand(
        IBotExceptionHandler exceptionHandler,
        IServerConfigurationAccessor configurationAccessor,
        IUserClient userClient) : base(exceptionHandler, configurationAccessor)
    {
        UserClient = userClient;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("ban", "Bans a user.")]
    public async Task ExecuteBanCommandAsync([Summary("discord-uuid", "The discord user id to ban")] string discordUuid)
    {
        await HandleCommandExecutionAsync(() => BanCommandInstructionsAsync(discordUuid));
    }

    protected override void ConfigureCommandSettings()
    {
    }

    private async Task BanCommandInstructionsAsync(string discordUuid)
    {
        if (await IsInvalidContextAsync(ValidChannelId))
        {
            return;
        }

        ulong id = await IsValidIdAsync(discordUuid);

        if (id == 0)
        {
            return;
        }

        var moderatedUser = new DiscordUser(id);

        await UserClient.BanUserAsync(moderatedUser);

        await FollowupAsync("Successfully banned provided user.");
    }
    
    private async Task<ulong> IsValidIdAsync(string uuid)
    {
        if (ulong.TryParse(uuid, out var userId))
        {
            return userId;
        }
        else
        {
            await FollowupAsync("Invalid uuid");
        }
        return 0;
    }
}