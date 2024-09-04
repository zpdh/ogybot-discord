using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ogybot.DataAccess.Controllers;
using ogybot.DataAccess.Entities;
using ogybot.Util;

namespace ogybot.Commands.Waitlist;

public class WaitlistAddCommand : BaseCommand
{
    private readonly WaitlistController _controller;

    public WaitlistAddCommand(WaitlistController controller)
    {
        _controller = controller;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("waitlist-add", "adds user to waitlist")]
    public async Task ExecuteCommandAsync([Summary("user", "user you're adding")] string username)
    {
        await DeferAsync();

        if (await ValidateChannelAsync(GuildChannels.LayoffsChannel)) return;

        if (username.Contains(' '))
        {
            await FollowupAsync("You cannot submit usernames with whitespaces");
            return;
        }

        var result = await _controller.AddPlayerAsync(new UserWaitlist { Username = username });

        var msg = result.Status
            ? $"Successfully added player '{result.Username}' to the wait list."
            : result.Error;

        await FollowupAsync(msg);
    }
}