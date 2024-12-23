using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Base;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Misc;

public class PingChiefsCommand : BaseCommand
{
    private ulong ValidChannelId { get; set; }

    public PingChiefsCommand(IBotExceptionHandler exceptionHandler, IGuildClient guildClient) : base(exceptionHandler, guildClient)
    {
    }

    protected override void ConfigureCommandSettings()
    {
        ValidChannelId = ServerConfiguration.WarChannel;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("chiefs", "Pings the 'Active Chief' role")]
    public async Task ExecuteCommandAsync()
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        await HandleCommandExecutionAsync(CommandInstructionsAsync);
    }

    private async Task CommandInstructionsAsync()
    {
        await FollowupAsync($"<@&{GuildRoleIds.ActiveChiefRole}>", allowedMentions: AllowedMentions.All);
    }
}