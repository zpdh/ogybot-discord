using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ogybot.Builders;
using ogybot.DataAccess.Sockets;

namespace ogybot;

public static class Program
{
    public static async Task Main()
    {
        var services = ServiceBuilder.Build();
        var config = services.GetRequiredService<IConfiguration>();
        var webSocketServer = services.GetRequiredService<ChatSocket>();
        var discordClient = services.GetRequiredService<DiscordSocketClient>();
        var interactionService = services.GetRequiredService<InteractionService>();

        discordClient.SetupInteraction(config, services, interactionService);
        await discordClient.SetupListenersAsync(webSocketServer, config);

        // Delay the task until program is closed
        await Task.Delay(-1);
    }
}