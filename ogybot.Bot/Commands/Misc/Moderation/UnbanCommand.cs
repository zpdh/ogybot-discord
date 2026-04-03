using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Core.Base;
using ogybot.Bot.Handlers;
using ogybot.Data.Clients;
using ogybot.Domain.Accessors;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Misc.Moderation;

public sealed class UnbanCommand : HighPermissionRequiredCommand
{
    private ulong ValidChannelId { get; set; }
    private readonly IUserClient UserClient;

    public UnbanCommand(
        IBotExceptionHandler exceptionHandler,
        IServerConfigurationAccessor configurationAccessor,
        IUserClient userClient) : base(exceptionHandler, configurationAccessor)
    {
        UserClient = userClient;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("unban", "Bans a user.")]
    public async Task ExecuteUnbanCommandAsync([Summary("discord-uuid", "The discord user id to unban")] string discordUuid)
    {
        await DeferAsync();
        await HandleCommandExecutionAsync(() => UnbanCommandInstructionsAsync(discordUuid));
    }

    protected override void ConfigureCommandSettings()
    {
    }

    private async Task UnbanCommandInstructionsAsync(string discordUuid)
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

        await UserClient.UnbanUserAsync(moderatedUser);

        await FollowupAsync("Successfully unbanned provided user.");
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