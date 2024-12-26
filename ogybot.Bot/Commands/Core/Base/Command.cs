using Discord.Interactions;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Domain.Accessors;
using ogybot.Domain.Entities.Configurations;

namespace ogybot.Bot.Commands.Core.Base;

public abstract class Command : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IBotExceptionHandler _botExceptionHandler;
    private readonly IServerConfigurationAccessor _configurationAccessor;

    protected ServerConfiguration ServerConfiguration { get; private set; }

    protected Command(IBotExceptionHandler exceptionHandler, IServerConfigurationAccessor configurationAccessor)
    {
        _botExceptionHandler = exceptionHandler;
        _configurationAccessor = configurationAccessor;
    }

    /// <summary>
    ///     Should be used as a second "constructor".
    ///     Configures internal fields in commands that can only be fetched during command execution, such as guild identifiers
    ///     and valid channels to use said command in.
    /// </summary>
    protected abstract void ConfigureCommandSettings();

    private async Task ConfigureCommandAsync()
    {
        ServerConfiguration = await _configurationAccessor.FetchServerConfigurationAsync(Context.Guild.Id);
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

    protected async Task<bool> IsInvalidChannelAsync(ulong channelId)
    {
        if (Context.Channel.Id == channelId) return false;

        await FollowupAsync(ErrorMessages.InvalidChannelError(channelId));
        return true;
    }
}