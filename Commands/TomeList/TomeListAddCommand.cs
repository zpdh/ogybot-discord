using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using test.Api.Controllers;
using test.Api.Entities;

namespace test.Commands.TomeList;

public abstract class TomeListAddCommand : ICommand
{
    private static readonly TomelistController Controller = new();

    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        var username = command.Data.Options.FirstOrDefault()!.Value;

        var result = await Controller.AddPlayerAsync(new UserTomelist { Username = username.ToString() });

        var msg = result.Status
            ? $"Successfully added player '{result.Username}' to the tome list."
            : $"Player '{result.Username}' is already on the tome list.";

        await command.FollowupAsync(msg);
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