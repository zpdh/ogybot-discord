using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Core.Base;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.CrossCutting.Accessors.Abstractions;

namespace ogybot.Bot.Commands.Misc;

public sealed class PingChiefsCommand : BaseCommand
{
    private ulong ValidChannelId { get; set; }

    public PingChiefsCommand(
        IBotExceptionHandler exceptionHandler,
        IServerConfigurationAccessor configurationAccessor) : base(exceptionHandler, configurationAccessor)
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
        await HandleCommandExecutionAsync(CommandInstructionsAsync);
    }

    private async Task CommandInstructionsAsync()
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        await FollowupAsync($"<@&{GuildRoleIds.ActiveChiefRole}>", allowedMentions: AllowedMentions.All);
    }
}