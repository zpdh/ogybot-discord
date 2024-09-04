using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using ogybot.DataAccess.Controllers;
using ogybot.DataAccess.Entities;
using ogybot.Util;

namespace ogybot.Commands.Waitlist;

public class WaitlistRemoveCommand : BaseRemoveCommand
{
    private readonly WaitlistController _controller;

    public WaitlistRemoveCommand(WaitlistController controller)
    {
        _controller = controller;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("waitlist-remove", "removes user from waitlist by name or index")]
    public async Task ExecuteCommandAsync([Summary("user-or-index", "the user or their index")] string input)
    {
        await DeferAsync();

        if (await ValidateChannelAndRolesAsync(GuildChannels.LayoffsChannel)) return;

        // Checks if input can be converted to an integer. If so, removes user by index instead of name.
        if (int.TryParse(input, out var index))
        {
            await RemoveByIndex(index);
            return;
        }

        await RemoveByName(input);
    }

    private async Task RemoveByName(string username)
    {
        var result = await _controller.RemovePlayerAsync(new UserWaitlist { Username = username });

        var msg = result.Status
            ? $"Successfully removed player '{result.Username}' from the wait list"
            : $"User '{result.Username}' is not on the wait list";

        await FollowupAsync(msg);
    }

    private async Task RemoveByIndex(int index)
    {
        var list = await _controller.GetWaitlistAsync();

        var username = list[index - 1].Username;
        var result = await _controller.RemovePlayerAsync(new UserWaitlist { Username = username });

        var msg = result.Status
            ? $"Successfully removed player '{result.Username}' from the wait list"
            : $"User '{result.Username}' is not on the wait list";

        await FollowupAsync(msg);
    }
}