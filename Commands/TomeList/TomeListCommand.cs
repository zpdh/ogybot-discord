using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using test.Api;
using test.Api.Controllers;
using test.SlashCommands;

namespace test.Commands.TomeList;

public class TomeListCommand : ICommand
{
    private static TomelistController _controller = new();
    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        
        var user = command.User;
        var list = await _controller.GetTomeQueueAsync();
        var description = "";
        var queueSize = "Players in queue: " + list.Count();

        var i = 1;
        foreach (var u in list)
        {
            description += i + ". " + u.Username.ToString() + "\n";
            i++;
        }
        
        var embedBuilder = new EmbedBuilder()
        .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
        .WithTitle($"Tome list")
        .WithDescription(description)
        .WithColor(Color.Teal)
        .WithCurrentTimestamp()
        .WithFooter(queueSize);

        await command.RespondAsync(embed: embedBuilder.Build());
        
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("tomelist")
                .WithDescription("Displays tome list");
            await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}