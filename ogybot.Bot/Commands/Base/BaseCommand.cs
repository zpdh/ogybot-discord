using Discord.Interactions;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Domain.Entities.Configurations;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Base;

public abstract class BaseCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IBotExceptionHandler _botExceptionHandler;
    private readonly IGuildClient _guildClient;

    protected BaseCommand(IBotExceptionHandler exceptionHandler, IGuildClient guildClient)
    {
        _botExceptionHandler = exceptionHandler;
        _guildClient = guildClient;
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

    public async Task<ServerConfiguration> GetServerConfigurationAsync()
    {
        var discordGuildId = GetGuildId();
        return await _guildClient.FetchConfigurationAsync(discordGuildId);
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