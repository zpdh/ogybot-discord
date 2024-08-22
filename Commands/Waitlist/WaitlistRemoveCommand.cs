using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using test.Api.Controllers;
using test.Api.Entities;

namespace test.Commands.Waitlist;

public abstract class WaitlistRemoveCommand : ICommand
{
    private static readonly WaitlistController Controller = new();
    
    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        var username = command.Data.Options.FirstOrDefault()!.Value;

        var result = await Controller.RemovePlayerAsync(new UserWaitlist { Username = username.ToString() });
        
        var msg = result.Status
            ? $"Successfully removed player '{result.Username}' from the wait list"
            : $"User '{result.Username}' is not on the wait list";
        
        await command.FollowupAsync(msg);
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("waitlist-remove")
                .WithDescription("Removes user from wait list")
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