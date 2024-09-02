using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using test.DataAccess.Controllers;
using test.DataAccess.Entities;

namespace test.Commands.Waitlist;

public abstract class WaitlistAddCommand : ICommand
{
    private static readonly WaitlistController Controller = new();

    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        var username = command.Data.Options.FirstOrDefault()!.Value;

        var result = await Controller.AddPlayerAsync(new UserWaitlist { Username = username.ToString() });

        var msg = result.Status
            ? $"Successfully added player '{result.Username}' to the wait list."
            : result.Error;

        await command.FollowupAsync(msg);
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("waitlist-add")
                .WithDescription("Adds user to wait list")
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