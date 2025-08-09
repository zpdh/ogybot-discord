using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Core.Base;
using ogybot.Bot.Handlers;
using ogybot.Data.Clients;
using ogybot.Domain.Accessors;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Misc.Moderation;

public sealed class ServerUnuteCommand : PermissionRequiredCommand
{
    private ulong ValidChannelId { get; set; }
    private readonly IGuildClient GuildClient;

    public ServerUnuteCommand(
        IBotExceptionHandler exceptionHandler,
        IServerConfigurationAccessor configurationAccessor,
        IGuildClient guildClient) : base(exceptionHandler, configurationAccessor)
    {
        GuildClient = guildClient;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("unmute", "Unmutes a user in the servers chat bridge.")]
    public async Task ExecuteMuteCommandAsync([Summary("discord-uuid", "The discord user id to unmute")] string discordUuid)
    {
        await HandleCommandExecutionAsync(() => MuteCommandInstructionsAsync(discordUuid));
    }

    protected override void ConfigureCommandSettings()
    {
        ValidChannelId = ServerConfiguration.ListeningChannel;
    }

    private async Task MuteCommandInstructionsAsync(string discordUuid)
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

        await GuildClient.UnmuteUserAsync(Context.Guild.Id, moderatedUser);

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
