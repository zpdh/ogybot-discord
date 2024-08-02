using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using test.Api;
using test.Services;

namespace test.Commands.Waitlist;

public class WaitlistRemoveCommand
{
    private static WaitlistController _controller = new();
    
    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        var username = command.Data.Options.FirstOrDefault().Value;

        var result = await _controller.RemovePlayerAsync(new UserWaitlist { Username = username.ToString() });
        
        await command.RespondAsync(result);
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("waitlist-remove")
                .WithDescription("Removes user from waitlist")
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