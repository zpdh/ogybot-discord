using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Core.Base;
using ogybot.Bot.Handlers;
using ogybot.Domain.Accessors;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Misc;

public sealed class LinkCommand : Command
{
    private readonly IUserClient UserClient;
    private ulong ValidChannelId { get; set; }
    private Guid WynnGuildId { get; set; }

    public LinkCommand(
        IBotExceptionHandler exceptionHandler,
        IServerConfigurationAccessor configurationAccessor, IUserClient userClient) : base(exceptionHandler, configurationAccessor)
    {
        UserClient = userClient;
    }

    protected override void ConfigureCommandSettings()
    {
        ValidChannelId = ServerConfiguration.BroadcastingChannel;
        WynnGuildId = ServerConfiguration.WynnGuildId;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("link", "Links minecraft account to discord.")]
    public async Task ExecuteLinkCommandAsync([Summary("mc-username", "The minecraft account to link to")] string mcUsername)
    {
        await DeferAsync();
        await HandleCommandExecutionAsync(() => LinkCommandInstructionsAsync(mcUsername));
    }

    private async Task LinkCommandInstructionsAsync(string mcUsername)
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        var linkUser = new LinkUser(mcUsername, Context.User.Id);
        await UserClient.LinkUserAsync(WynnGuildId, linkUser);

        await FollowupAsync("Successfully linked to account");
    }
}