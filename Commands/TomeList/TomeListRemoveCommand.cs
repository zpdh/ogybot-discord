using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using test.Api.Controllers;
using test.Api.Entities;

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

        var listOfUsers = username!.Split(", ")
            .Distinct()
            .ToList();

        var returnList = new List<string>();
        var statusList = new List<bool>();

        foreach (var user in listOfUsers)
        {
            var result = await Controller.RemovePlayerAsync(new UserTomelist { Username = user });

            returnList.Add(result.Username);
            statusList.Add(result.Status);
        }

        if (statusList.Contains(false))
        {
            await command.FollowupAsync($"One or more players are invalid. Check the list for an update and try again");
            return;
        }

        var users = returnList.Aggregate("", (current, user) => current + ($"'{user}'" + ", "));

        // [..^n] removes the last n characters of an array
        var msg = returnList.Count != 0
            ? $"Successfully removed players {users[..^2]} from the tome list."
            : "One or more players provided are not on the tome list";

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