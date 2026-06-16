using Discord;
using Discord.Interactions;
using ogybot.DataAccess.Controllers;
using ogybot.Util;

namespace ogybot.Commands.AspectList;

/// <summary>
/// Command used to increment an aspect from a user
/// </summary>
public class IncrementAspectCommand : BaseAddCommand
{
    private readonly AspectsController _controller;

    public IncrementAspectCommand(AspectsController controller)
    {
        _controller = controller;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("aspectlist-increment", "increments an aspect by the user's name")]
    public async Task ExecuteCommandAsync([Summary("user", "user you're adding")] string username)
    {
        if (await ValidateChannelAndRolesAsync(GuildChannels.RaidsChannel)) return;

        if (username.Contains(' ') && !username.Contains(','))
        {
            await FollowupAsync("You cannot submit usernames with whitespaces");
            return;
        }

        var listOfUsers = username.Split(",")
            .Distinct()
            .Select(user => user.Trim())
            .ToList();

        var result = await _controller.IncrementPlayersAspectsAsync(listOfUsers);

        var users = listOfUsers.Aggregate("", (current, user) => current + ($"'{user}'" + ", "));

        // [..^n] removes the last n characters of an array
        var msg = result.Status
            ? $"Successfully incremented an aspect from players {users[..^2]} from the aspect list."
            : $"There was an error incrementing the players {users[..^2]}. Perhaps the API is down?";

        await FollowupAsync(msg);
    }
}
