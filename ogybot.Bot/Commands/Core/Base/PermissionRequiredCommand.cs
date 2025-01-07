using Discord;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Domain.Accessors;

namespace ogybot.Bot.Commands.Core.Base;

public abstract class PermissionRequiredCommand : Command
{

    protected PermissionRequiredCommand(
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

        if (await IsInvalidChannelAsync(channelId)) return true;

        return await UserHasNoRolesAsync();

    }

    private async Task<bool> UserHasNoRolesAsync()
    {
        // This method should always be called before a command, therefore this shouldn't present any issues.
        var validRoles = ServerConfiguration.PrivilegedRoles;

        var user = Context.User as IGuildUser;

        var userHasValidRole = user!.RoleIds.Any(role => validRoles.Contains(role));

        if (userHasValidRole) return false;

        await FollowupAsync(ErrorMessages.NoPermissionError);

        return true;
    }
}