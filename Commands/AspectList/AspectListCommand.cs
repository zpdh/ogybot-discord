using System.Globalization;
using Discord;
using Discord.Interactions;
using ogybot.DataAccess.Controllers;
using ogybot.Util;

namespace ogybot.Commands.AspectList;

/// <summary>
/// Command used to display the aspect list.
/// </summary>
public class AspectListCommand : BaseCommand
{
    private readonly AspectsController _controller;

    public AspectListCommand(AspectsController controller)
    {
        _controller = controller;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("aspectlist", "displays aspect list")]
    public async Task ExecuteCommandAsync()
    {
        if (await ValidateChannelAsync(GuildChannels.RaidsChannel)) return;

        var user = Context.User;

        var aspectEnum = await _controller.GetAspectListAsync();
        var list = aspectEnum!.ToList();

        var description = "";

        var queueSize = "Players in queue: " + list.Count;

        for (var i = 1; i <= list.Count; i++)
        {
            var player = list[i - 1];
            description += $"{i}. {player.Username} - {player.Aspects.ToString(CultureInfo.InvariantCulture)}\n";
        }

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle("Aspect list")
            .WithDescription(description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter(queueSize);

        await FollowupAsync(embed: embedBuilder.Build());
    }
}