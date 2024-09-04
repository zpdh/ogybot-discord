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

        var user = Context.User as IGuildUser;

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

    private async Task<bool> ValidateChannelAndRoles(IGuildUser user)
    {
        if (Context.Channel.Id != GuildChannels.TomeChannel)
        {
            await FollowupAsync(ErrorMessages.InvalidChannelError);
            return true;
        }

        var roles = user
            .RoleIds
            .Where(role => role is 1060001967868485692 or 810680884193787974 or 1097935496442810419);

        if (roles.Any()) return false;

        await FollowupAsync(ErrorMessages.NoPermissionError);
        return true;
    }
}