using Discord;
using ogybot.Util;

namespace ogybot.Commands;

public class BaseRemoveCommand : BaseCommand
{
    protected async Task<bool> ValidateChannelAndRolesAsync(ulong channelId)
    {
        if (await IsInvalidChannelAsync(channelId)) return true;

        var user = Context.User as IGuildUser;

        var roles = user!
            .RoleIds
            .Where(role => role is 1060001967868485692 or 810680884193787974 or 1097935496442810419);

        if (!roles.Any())
        {
            await FollowupAsync(ErrorMessages.NoPermissionError);
            return true;
        }

        return false;
    }
}