using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using ogybot.DataAccess.Controllers;

namespace ogybot.Commands.TomeList;

public class TomeListCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly TomelistController _controller = new();

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("tomelist", "displays the tome list")]
    public async Task ExecuteCommandAsync()
    {
        await DeferAsync();

        var user = Context.User;

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