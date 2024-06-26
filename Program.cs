using Discord.WebSocket;
using Discord;
using Discord.Net;
using Newtonsoft.Json;
using test.Loggers;
using test.Login;
using test.Commands;

public class Program
{
    private static Client? _discordClient;

    public static async Task Main()
    {

        _discordClient = new(new DiscordSocketClient());

        CommandController commands = new(_discordClient.client);

        _discordClient.Log();

        await _discordClient.Login();

        _discordClient.client.Ready += commands.Client_Ready;
        _discordClient.client.SlashCommandExecuted += commands.SlashCommandHandler;

        await Task.Delay(-1);
    }
}
