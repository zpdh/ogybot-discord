using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
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

    public TomeListAddCommand(TomelistController controller)
    {
        _controller = controller;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("tomelist-add", "Adds user to tome list")]
    public async Task ExecuteCommandAsync([Summary("username", "the user's name you're adding")] string username)
    {
        await DeferAsync();

        if (await ValidateChannelAsync(GuildChannels.TomeChannel)) return;

        if (username.Contains(' '))
        {
            await FollowupAsync("You cannot submit usernames with whitespaces");
            return;
        }

        var result = await _controller.AddPlayerAsync(new UserTomelist { Username = username.ToString() });

        var msg = result.Status
            ? $"Successfully added player '{result.Username}' to the tome list."
            : result.Error;

        await FollowupAsync(msg);
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("tomelist-add")
                .WithDescription("Adds user to tome list")
                .AddOption("username", ApplicationCommandOptionType.String, "User you're adding", true);
            await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}