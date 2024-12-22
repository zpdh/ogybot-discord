using Discord;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Base;

public abstract class BasePermissionRequiredCommand : BaseCommand
{
    private readonly ICollection<ulong> _validRoleIds;

    protected BasePermissionRequiredCommand(IBotExceptionHandler exceptionHandler, IGuildClient guildClient) : base(exceptionHandler, guildClient)
    {
        _validRoleIds = ServerConfiguration.PrivilegedRoles;
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
        var user = Context.User as IGuildUser;

        var userHasValidRole = user!.RoleIds.Any(role => _validRoleIds.Contains(role));

        if (userHasValidRole) return false;

        await FollowupAsync(ErrorMessages.NoPermissionError);

        return true;
    }
}