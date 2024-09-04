using Discord;
using Discord.Interactions;
using ogybot.DataAccess.Controllers;
using ogybot.Util;

namespace ogybot.Commands.Waitlist;

/// <summary>
/// Displays the wait list.
/// </summary>
public class WaitlistCommand : BaseCommand
{
    private readonly WaitlistController _controller;

    public WaitlistCommand(WaitlistController controller)
    {
        _controller = controller;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("waitlist", "displays the wait list")]
    public async Task ExecuteCommandAsync()
    {
        await DeferAsync();

        if (await ValidateChannelAsync(GuildChannels.LayoffsChannel)) return;

        var user = Context.User;

        var list = await _controller.GetWaitlistAsync();

        var description = "";

        var queueSize = "Players in queue: " + list.Count;

        for (var i = 1; i <= list.Count; i++)
        {
            description += i + ". " + list[i - 1].Username + "\n";
        }

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle("Wait list")
            .WithDescription(description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter(queueSize);

        await FollowupAsync(embed: embedBuilder.Build());
    }
}