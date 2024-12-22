using Discord.Interactions;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Domain.Entities.Configurations;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Base;

// TODO: THIS WILL NOT WORK. Need to move server configuration initialization to the command execution scope. Figure out how to do things from there.
public abstract class BaseCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IBotExceptionHandler _botExceptionHandler;

    protected readonly ServerConfiguration ServerConfiguration;

    protected BaseCommand(IBotExceptionHandler exceptionHandler, IGuildClient guildClient)
    {
        _botExceptionHandler = exceptionHandler;
        ServerConfiguration = GetServerConfigurationAsync(guildClient).GetAwaiter().GetResult();
    }

    protected async Task TryExecutingCommandInstructionsAsync(Func<Task> command)
    {
        try
        {
            await command();
        }
        catch (Exception e)
        {
            await _botExceptionHandler.HandleAsync(Context, e);
        }
    }

    private async Task<ServerConfiguration> GetServerConfigurationAsync(IGuildClient guildClient)
    {
        var discordGuildId = GetGuildId();
        return await guildClient.FetchConfigurationAsync(discordGuildId);
    }

    private ulong GetGuildId()
    {
        return Context.Guild.Id;
    }

    protected async Task<bool> IsInvalidChannelAsync(ulong channelId)
    {
        if (Context.Channel.Id == channelId) return false;

        await FollowupAsync(ErrorMessages.InvalidChannelError);
        return true;
    }
}