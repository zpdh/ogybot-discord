using Discord.Interactions;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;

namespace ogybot.Bot.Commands.Base;

public abstract class BaseCommand : InteractionModuleBase<SocketInteractionContext>
{
    // Add to dependency injection later
    private readonly ExceptionHandler _exceptionHandler = new ExceptionHandler();

    protected async Task<bool> IsInvalidChannelAsync(ulong channelId)
    {
        if (Context.Channel.Id == channelId) return false;

        await FollowupAsync(ErrorMessages.InvalidChannelError);
        return true;
    }

    protected async Task TryExecutingCommandInstructionsAsync(Func<Task> command)
    {
        try
        {
            await command();
        }
        catch (Exception e)
        {
            await ExceptionHandler.HandleAsync(Context, e);
        }
    }
}