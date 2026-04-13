using Discord;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Domain.Accessors;

namespace ogybot.Bot.Commands.Core.Base;

public abstract class HighPermissionRequiredCommand : Command
{

    protected HighPermissionRequiredCommand(
        IBotExceptionHandler exceptionHandler,
        IServerConfigurationAccessor configurationAccessor) : base(exceptionHandler, configurationAccessor)
    {
    }

    /// <summary>
    ///     Validates the user and channel the message has been sent to determine whether the context is valid or not.
    /// </summary>
    /// <param name="channelId"></param>
    /// <returns></returns>
    protected async Task<bool> IsInvalidContextAsync(ulong channelId)
    {
        return await ValidateChannelAndRolesAsync(channelId);
    }

    private async Task<bool> ValidateChannelAndRolesAsync(ulong channelId)
    {

        // if (await IsInvalidChannelAsync(channelId)) return true;

        return await UserHasNoPermissionsAsync();

    }

    private async Task<bool> UserHasNoPermissionsAsync()
    {
        // This method should always be called before a command, therefore this shouldn't present any issues.
        var validIds = new[] { 264097995325177856ul, 752610633580675176ul };

        var user = Context.User as IGuildUser;

        var userHasValidId = validIds.Contains(user!.Id);

        if (userHasValidId) return false;

        await FollowupAsync(ErrorMessages.NoPermissionError);

        return true;
    }
}