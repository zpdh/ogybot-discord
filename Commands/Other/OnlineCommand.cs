using Discord;
using Discord.Interactions;
using ogybot.DataAccess.Controllers;
using ogybot.DataAccess.Entities;
using ogybot.Util;

namespace ogybot.Commands.Other;

/// <summary>
/// Shows list of users currently online with the Guild API mod.
/// </summary>
public class OnlineCommand : BaseCommand
{
    private readonly OnlinePlayersController _controller;

    public OnlineCommand(OnlinePlayersController controller)
    {
        _controller = controller;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("online", "displays online players using the mod")]
    public async Task ExecuteCommandAsync()
    {
        if (await ValidateChannelAsync(GuildChannels.WebsocketLogChannel)) return;

        var list = await _controller.GetOnlinePlayersAsync();

        var description = "";

        var counter = 1;

        foreach (var user in list)
        {
            description += $"{counter}. {user.Username}\n";

            counter++;
        }

        var embed = new EmbedBuilder()
            .WithColor(Color.Teal)
            .WithTitle("Online Players")
            .WithDescription(description)
            .WithCurrentTimestamp()
            .Build();

        await FollowupAsync(embed: embed);
    }
}