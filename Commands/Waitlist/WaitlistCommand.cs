using Discord;
using Discord.Interactions;
using Discord.WebSocket;
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
        if (await IsInvalidChannelAsync(GuildChannels.LayoffsChannel)) return;

        var user = Context.User;

        var list = await _controller.GetWaitlistAsync();

        var description = "";

        var queueSize = "Players in queue: " + list.Count;

        var counter = 1;

        foreach (var userWaitlist in list)
        {
            description += $"{counter}. {userWaitlist.Username}\n";

            counter++;
        }

        var embed = CreateEmbed(user, description, queueSize);

        await FollowupAsync(embed: embed);
    }

    private static Embed CreateEmbed(SocketUser user, string description, string queueSize)
    {
        var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle("Wait list")
            .WithDescription(description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter(queueSize);

        return embedBuilder.Build();
    }
}