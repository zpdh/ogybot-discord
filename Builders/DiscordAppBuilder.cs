using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using test.Login;
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

    public static void AddCommands(this DiscordSocketClient discordClient)
    {
        CommandService commands = new(discordClient);
        
        discordClient.Ready += commands.Client_Ready;
        discordClient.SlashCommandExecuted += commands.SlashCommandHandler;
    }

    private static async Task ConnectAsync(this DiscordSocketClient client, IConfiguration configuration)
    {
        var branch = configuration.GetBranch();
        var token = configuration.GetValue<string>($"Token:{branch}");

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();
    }
}