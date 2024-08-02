using Discord.WebSocket;
using Discord;
using Discord.Net;
using Newtonsoft.Json;
using test.Loggers;
using test.Login;
using test.Services;
using test.SlashCommands;
using Discord.Commands;
using test.Api;

namespace test;

public class Program
{
    private static Client? _discordClient;

    public static async Task Main()
    {
        //Data instances
        
        
        // Discord client stuff downwards
        
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