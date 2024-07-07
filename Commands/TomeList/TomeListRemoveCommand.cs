using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using test.Api;
using test.Services;
using test.SlashCommands;

namespace test.Commands.TomeList;

public class TomeListRemoveCommand : ICommand
{
    private static TomeController _controller = new();
    
    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        var username = command.Data.Options.FirstOrDefault().Value;

        var result = await _controller.RemovePlayerAsync(new User { UserName = username.ToString() });
        
        await command.RespondAsync(result);
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