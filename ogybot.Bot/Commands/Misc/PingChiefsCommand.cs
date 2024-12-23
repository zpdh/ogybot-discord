using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Base;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Misc;

public class PingChiefsCommand : BaseCommand
{

    public PingChiefsCommand(IBotExceptionHandler exceptionHandler, IGuildClient guildClient) : base(exceptionHandler, guildClient)
    {
    }

    private ulong ValidChannelId { get; set; }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("chiefs", "Pings the 'Active Chief' role")]
    public async Task ExecuteChiefsCommandAsync()
    {
        var serverConfig = await GetServerConfigurationAsync();
        ValidChannelId = serverConfig.RaidsChannel;

        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        await FollowupAsync($"<@&{GuildRoleIds.ActiveChiefRole}>", allowedMentions: AllowedMentions.All);
    }
}