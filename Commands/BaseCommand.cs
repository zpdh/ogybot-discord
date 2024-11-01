using Discord;
using Discord.Interactions;
using ogybot.Util;

namespace ogybot.Commands;

public abstract class BaseCommand : InteractionModuleBase<SocketInteractionContext>
{
    protected async Task<bool> IsInvalidChannelAsync(ulong channelId)
    {
        if (Context.Channel.Id == channelId) return false;

        await FollowupAsync(ErrorMessages.InvalidChannelError);
        return true;
    }
}