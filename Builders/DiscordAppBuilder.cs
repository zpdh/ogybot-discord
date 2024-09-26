using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using ogybot.DataAccess.Sockets;
using ogybot.Services;

namespace ogybot.Builders;

/// <summary>
/// Class responsible for the setup of a <see cref="DiscordSocketClient"/>.
/// </summary>
public static class DiscordAppBuilder
{
    public static async Task<DiscordSocketClient> SetupDiscordClientAsync(IConfiguration configuration)
    {
        var discordConfig = new DiscordSocketConfig
        {
            UseInteractionSnowflakeDate = false
        };

        var discordClient = new DiscordSocketClient(discordConfig);

        await discordClient.ConnectAsync(configuration);

        discordClient.Log += LoggerService.Log;

        return discordClient;
    }

    public static void SetupInteraction(
        this DiscordSocketClient client,
        IConfiguration configuration,
        IServiceProvider services,
        InteractionService interactionService)
    {
        // Create interaction service and register it

        client.InteractionCreated += async (interaction) => {
            await interaction.DeferAsync();

            var context = new SocketInteractionContext(client, interaction);
            var result = await interactionService.ExecuteCommandAsync(context, services); // Change once DI gets added

            if (!result.IsSuccess)
            {
                // Pings me and logs error in console for debugging
                await context.Channel.SendMessageAsync(
                    "An error occurred executing this command, <@264097995325177856> HELP!!!",
                    allowedMentions: AllowedMentions.All);
                Console.WriteLine(result.Error);
            }
        };

        // Get guild ID
        var branch = configuration["Branch"];
        var id = configuration.GetValue<ulong>($"ServerIds:{branch}");

        client.Ready += async () => {
            await interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), services);

            await interactionService.RegisterCommandsToGuildAsync(id);
        };
    }

    public static async Task SetupListenerAsync(
        this DiscordSocketClient client,
        ChatServer server,
        IConfiguration configuration)
    {
        var channelId = configuration.GetValue<ulong>("LoggingChannelId");

        if (await client.GetChannelAsync(channelId) is IMessageChannel channel)
        {
            await server.StartServerAsync(channel);
        }

        Console.WriteLine("Could not find logging channel.");
    }

    private static async Task ConnectAsync(this DiscordSocketClient client, IConfiguration configuration)
    {
        var branch = configuration["Branch"];
        var token = configuration[$"Tokens:{branch}"];

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();
    }
}