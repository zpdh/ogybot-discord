using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ogybot.Builders;

namespace ogybot;

public static class Program
{
    public static async Task Main()
    {
        var services = ServiceBuilder.Build();
        var config = services.GetRequiredService<IConfiguration>();
        var discordClient = services.GetRequiredService<DiscordSocketClient>();
        var interactionService = services.GetRequiredService<InteractionService>();

        discordClient.SetupInteractionAsync(config, services, interactionService);

        // Delay the task until program is closed
        await Task.Delay(-1);
    }
}

