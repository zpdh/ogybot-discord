using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using test.Api.Controllers;
using test.Api.Entities;

namespace test.Commands.AspectList;

public class DecrementAspectCommand
{
    private static readonly AspectsController Controller = new();

    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        var username = command.Data.Options.FirstOrDefault()!.Value as string;

        if (username!.Contains(' ') && !username.Contains(','))
        {
            await command.FollowupAsync("You cannot submit usernames with whitespaces");
            return;
        }

        var listOfUsers = username!.Split(",")
            .Distinct()
            .Select(user => user.Trim())
            .ToList();

        var result = await Controller.DecrementPlayersAspectsAsync(listOfUsers);

        var users = listOfUsers.Aggregate("", (current, user) => current + ($"'{user}'" + ", "));

        // [..^n] removes the last n characters of an array
        var msg = result.Status
            ? $"Successfully decremented an aspect from players {users[..^2]} from the aspect list."
            : $"There was an error decrementing the players {users[..^2]}. Perhaps the API is down?";

        await command.FollowupAsync(msg);
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("aspectlist-decrement")
                .WithDescription("Decrements aspect count by 1")
                .AddOption("user-list", ApplicationCommandOptionType.String, "Users you're decrementing", true);
            await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}