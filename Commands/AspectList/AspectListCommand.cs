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
        if (await IsInvalidChannelAsync(GuildChannels.RaidsChannel)) return;

        var user = Context.User;

        var aspectEnum = await _controller.GetAspectListAsync();
        var list = aspectEnum!.ToList();

        var description = "";

        var queueSize = "Players in queue: " + list.Count;

        var counter = 1;

        foreach (var player in list)
        {
            description += $"{counter}. {player.Username} - {player.Aspects}\n";

            counter++;
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