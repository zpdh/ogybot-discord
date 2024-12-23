using Discord.Interactions;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Communication.Exceptions;
using ogybot.Domain.Entities.Configurations;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Core.Base;

public abstract class BaseCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IBotExceptionHandler _botExceptionHandler;
    private readonly IGuildClient _guildClient;

    protected ServerConfiguration ServerConfiguration { get; private set; }

    protected BaseCommand(IBotExceptionHandler exceptionHandler, IGuildClient guildClient)
    {
        _botExceptionHandler = exceptionHandler;
        _guildClient = guildClient;
    }

    /// <summary>
    /// Should be used as a second "constructor".
    /// Configures internal fields in commands that can only be fetched during command execution, such as valid channels to use said command in and guild identifiers.
    /// </summary>
    protected abstract void ConfigureCommandSettings();

    private async Task ConfigureCommandAsync()
    {
        ServerConfiguration = await FetchServerConfigurationAsync();
        ConfigureCommandSettings();
    }

    protected async Task HandleCommandExecutionAsync(Func<Task> command, bool requiresConfiguration = true)
    {
        try
        {
            if (requiresConfiguration)
            {
                await ConfigureCommandAsync();
            }

            await command();
        }
        catch (Exception e)
        {
            await _botExceptionHandler.HandleAsync(Context, e);
        }
    }

    private async Task<ServerConfiguration> FetchServerConfigurationAsync()
    {
        var discordGuildId = GetGuildId();
        var serverConfiguration = await _guildClient.FetchConfigurationAsync(discordGuildId);

        if (serverConfiguration is null)
        {
            throw new FetchingException(ExceptionMessages.GuildNotConfigured);
        }

        return serverConfiguration;
    }

    private ulong GetGuildId()
    {
        return Context.Guild.Id;
    }

    protected async Task<bool> IsInvalidChannelAsync(ulong channelId)
    {
        if (Context.Channel.Id == channelId) return false;

        await FollowupAsync(ErrorMessages.InvalidChannelError(channelId));
        return true;
    }
}