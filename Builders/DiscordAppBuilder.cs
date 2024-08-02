using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using test.Services;

namespace test.Builders;

public static class DiscordAppBuilder
{
    public static async Task<DiscordSocketClient> SetupDiscordClientAsync(IConfiguration configuration)
    {
        var discordClient = new DiscordSocketClient();

        await discordClient.ConnectAsync(configuration);

        discordClient.Log += LoggerService.Log;

        return discordClient;
    }

    public static void AddCommands(this DiscordSocketClient discordClient, IConfiguration configuration)
    {
        var branch = configuration["Branch"];
        var id = configuration.GetValue<ulong>($"ServerIds{branch}");
        
        CommandService commands = new(discordClient, id);
        
        discordClient.Ready += commands.Client_Ready;
        discordClient.SlashCommandExecuted += commands.SlashCommandHandler;
    }

    private static async Task ConnectAsync(this DiscordSocketClient client, IConfiguration configuration)
    {
        var branch = configuration["Branch"];
        var token = configuration[$"Tokens:{branch}"];

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();
    }
}