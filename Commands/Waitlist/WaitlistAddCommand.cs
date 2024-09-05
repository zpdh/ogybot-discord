using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using ogybot.DataAccess.Controllers;
using ogybot.DataAccess.Entities;
using ogybot.Util;

namespace ogybot.Commands.Waitlist;

public class WaitlistAddCommand : BaseCommand
{
    private readonly WaitlistController _controller;
    private readonly string _allowedCharacters;

    public WaitlistAddCommand(WaitlistController controller, IConfiguration configuration)
    {
        _controller = controller;
        _allowedCharacters = configuration["AllowedCharacters"]!;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("waitlist-add", "adds user to waitlist")]
    public async Task ExecuteCommandAsync([Summary("user", "user you're adding")] string username)
    {
        if (await ValidateChannelAsync(GuildChannels.LayoffsChannel)) return;

        if(username.Any(character => !_allowedCharacters.Contains(character)))
        {
            await FollowupAsync("You cannot submit usernames with one or more of those characterss");
            return;
        }

        var result = await _controller.AddPlayerAsync(new UserWaitlist { Username = username });

        var msg = result.Status
            ? $"Successfully added player '{result.Username}' to the wait list."
            : result.Error;

        await FollowupAsync(msg);
    }
}