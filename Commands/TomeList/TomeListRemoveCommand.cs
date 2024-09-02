using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using test.DataAccess.Controllers;
using test.DataAccess.Entities;

namespace test.Commands.TomeList;

public abstract class TomeListRemoveCommand : ICommand
{
    private static readonly TomelistController Controller = new();

    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        var username = command.Data.Options.FirstOrDefault()!.Value as string;

        if (username!.Contains(' ') && !username.Contains(','))
        {
            await command.FollowupAsync("You cannot submit usernames with whitespaces");
            return;
        }

        var nameList = username!.Split(", ")
            .Distinct()
            .Select(user => user.Trim())
            .ToList();

        var responseList = new List<Response>();

        foreach (var user in nameList)
        {
            var result = await Controller.RemovePlayerAsync(new UserTomelist { Username = user });

            responseList.Add(result);
        }

        var statusList = responseList
            .Select(response => response.Status);

        var errorList = responseList
            .Select(response => response.Error)
            .Distinct()
            .Select(error => error is not null);

        if (statusList.Contains(false))
        {
            var formattedErrorList = errorList.Aggregate("", (current, error) => current + $"'{error}'" + ", ");

            await command.FollowupAsync($"One or multiple errors occurred: {formattedErrorList}");
            return;
        }

        var users = nameList.Aggregate("", (current, user) => current + ($"'{user}'" + ", "));

        // [..^n] removes the last n characters of an array
        var msg = $"Successfully removed players {users[..^2]} from the tome list.";

        await command.FollowupAsync(msg);
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("tomelist-remove")
                .WithDescription("Removes user from tome list")
                .AddOption("username", ApplicationCommandOptionType.String, "User you're removing", true);
            await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}