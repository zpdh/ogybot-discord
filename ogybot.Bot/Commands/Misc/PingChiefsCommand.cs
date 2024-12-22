using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Base;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Misc;

public class PingChiefsCommand : BaseCommand
{
    private readonly ulong _validChannelId;

    public PingChiefsCommand(IBotExceptionHandler exceptionHandler, IGuildClient guildClient) : base(exceptionHandler, guildClient)
    {
        _validChannelId = ServerConfiguration.WarChannel;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("chiefs", "Pings the 'Active Chief' role")]
    public async Task ExecuteChiefsCommandAsync()
    {
        if (await IsInvalidChannelAsync(_validChannelId))
        {
            return;
        }

        await FollowupAsync($"<@&{GuildRoleIds.ActiveChiefRole}>", allowedMentions: AllowedMentions.All);
    }
}