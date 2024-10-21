using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ogybot.DataAccess.Controllers;
using ogybot.DataAccess.Entities;
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

        var embed = await CreateEmbedAsync();

        await FollowupAsync(embed: embed);
    }

    private async Task<Embed> CreateEmbedAsync()
    {
        // Create class to store this info later
        var (user, queueSize, description) = await GetEmbedContentAsync();

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle("Wait list")
            .WithDescription(description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter(queueSize);

        return embedBuilder.Build();
    }

    private async Task<(SocketUser user, string queueSize, string description)> GetEmbedContentAsync()
    {
        var user = Context.User;

        var list = await _controller.GetWaitlistAsync();

        var queueSize = "Players in queue: " + list.Count;

        var description = CreateEmbedDescription(list);

        return (user, queueSize, description);
    }

    private static string CreateEmbedDescription(List<UserWaitlist> list)
    {
        var description = "";

        var counter = 1;

        foreach (var userWaitlist in list)
        {
            description += $"{counter}. {userWaitlist.Username}\n";

            counter++;
        }

        return description;
    }
}