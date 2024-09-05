using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using ogybot.DataAccess.Controllers;
using ogybot.DataAccess.Entities;
using ogybot.Util;

namespace ogybot.Commands.TomeList;

/// <summary>
/// Adds the specified user to the tome list
/// </summary>
public class TomeListAddCommand : BaseCommand
{
    private readonly TomelistController _controller;
    private readonly string _allowedCharacters;

    public TomeListAddCommand(TomelistController controller, IConfiguration configuration) {
        _controller = controller;
        _allowedCharacters = configuration["ValidCharacters"]!;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("tomelist-add", "Adds user to tome list")]
    public async Task ExecuteCommandAsync([Summary("username", "the user's name you're adding")] string username)
    {
        if (await ValidateChannelAsync(GuildChannels.TomeChannel)) return;

        if(username.Any(character => !_allowedCharacters.Contains(character)))
        {
                await FollowupAsync("You cannot submit usernames with one or more of those characters");
                return;
        }

        var result = await _controller.AddPlayerAsync(new UserTomelist { Username = username });

        var msg = result.Status
            ? $"Successfully added player '{result.Username}' to the tome list."
            : result.Error;

        await FollowupAsync(msg);
    }
}