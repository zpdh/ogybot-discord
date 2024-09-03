using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using ogybot.Services;
using ogybot.Commands;
using ogybot.Commands.TomeList;
using ogybot.Commands.Waitlist;

namespace ogybot.Builders;

/// <summary>
/// Class responsible for the setup of a <see cref="DiscordSocketClient"/>.
/// </summary>
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
        var id = configuration.GetValue<ulong>($"ServerIds:{branch}");

        CommandService commands = new(discordClient, id, CommandDictionaryBuilder.Build());

        discordClient.Ready += commands.Client_Ready;
        // Only uncomment if you need to instance commands. Takes up startup time

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