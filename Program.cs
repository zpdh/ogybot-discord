using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using test.Api;
using test.Login;
using test.Services;

namespace test;

public class Program
{
    private static Client? _discordClient;

    public static async Task Main()
    {
        _discordClient = new Client(new DiscordSocketClient());

        CommandsControllerService commands = new(_discordClient.client);

        _discordClient.Log();

        await _discordClient.Login();

        /*
         * Have to instance command below every bot migration.
         * It is responsible for creating the commands and only
         * needs to run once.
         */

        //_discordClient.client.Ready += commands.Client_Ready;

        _discordClient.client.SlashCommandExecuted += commands.SlashCommandHandler;
        
        await Task.Delay(-1);
    }
}