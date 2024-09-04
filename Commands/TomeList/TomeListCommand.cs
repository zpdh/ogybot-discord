using Discord;
using Discord.Interactions;
using ogybot.DataAccess.Controllers;
using ogybot.Util;

namespace ogybot.Commands.TomeList;

/// <summary>
/// Displays the tome list.
/// </summary>
public class TomeListCommand : BaseCommand
{
    private readonly TomelistController _controller;

    public TomeListCommand(TomelistController controller)
    {
        _controller = controller;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("tomelist", "displays the tome list")]
    public async Task ExecuteCommandAsync()
    {
        await DeferAsync();

        var user = Context.User;

        // Checks if user is in correct channel and has perms to execute the command
        if (await ValidateChannelAsync(GuildChannels.TomeChannel)) return;

        var list = await _controller.GetTomelistAsync();

        var description = "";

        var queueSize = "Players in queue: " + list.Count;

        for (var i = 1; i <= list.Count; i++)
        {
            description += i + ". " + list[i - 1].Username + "\n";
        }

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle("Tome list")
            .WithDescription(description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter(queueSize);

        await FollowupAsync(embed: embedBuilder.Build());
    }
}