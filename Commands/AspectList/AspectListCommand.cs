using System.Globalization;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using test.Api.Controllers;

namespace test.Commands.AspectList;

public class AspectListCommand : ICommand
{
    private static readonly AspectsController Controller = new();

    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        var user = command.User;

        var aspectEnum = await Controller.GetAspectListAsync();
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

        await command.FollowupAsync(embed: embedBuilder.Build());
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("aspectlist")
                .WithDescription("Displays aspect list");
            await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}