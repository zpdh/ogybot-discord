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
        var interactionService = services.GetRequiredService<InteractionService>();

        while (true)
        {
            try
            {
                var discordClient = services.GetRequiredService<DiscordSocketClient>();

                discordClient.SetupInteraction(config, services, interactionService);

                await discordClient.SetupListenersAsync(webSocketServer);

                // Delay the task until program is closed
                await Task.Delay(-1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType()}: {e.Message}");

                // Slight delay to avoid overflowing
                await Task.Delay(5000);
            }
        }
    }
}