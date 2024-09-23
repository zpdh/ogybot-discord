using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ogybot.Builders;
using ogybot.DataAccess.Clients;
using ogybot.DataAccess.Controllers;
using ogybot.DataAccess.Sockets;

namespace ogybot.Extensions;

public static class ServiceExtensions {
    public static void AddServices(this ServiceCollection services) {
        services.AddConfiguration();
        services.AddDiscordClient();
        services.AddInteractionService();
        services.AddClients();
        services.AddControllers();
        services.AddSockets();
    }

    private static void AddConfiguration(this ServiceCollection services) {
        services.AddSingleton(AppConfigurationBuilder.Build());
    }

    private static void AddDiscordClient(this ServiceCollection services) {
        services.AddSingleton<DiscordSocketClient>(provider => {
            var config = provider.GetRequiredService<IConfiguration>();

            var discordClient = DiscordAppBuilder.SetupDiscordClientAsync(config).GetAwaiter().GetResult();

            return discordClient;
        });
    }

    private static void AddInteractionService(this ServiceCollection services) {
        services.AddSingleton<InteractionService>(provider => {
            var client = provider.GetRequiredService<DiscordSocketClient>();

            return new InteractionService(client.Rest);
        });
    }

    private static void AddClients(this ServiceCollection services) {
        services.AddSingleton<TomeClient>();
        services.AddSingleton<WaitlistClient>();
        services.AddSingleton<AspectClient>();
    }

    private static void AddControllers(this ServiceCollection services) {
        services.AddSingleton<TomelistController>();
        services.AddSingleton<WaitlistController>();
        services.AddSingleton<AspectsController>();
    }

    private static void AddSockets(this ServiceCollection services) {
        services.AddSingleton<ChatSocket>();
    }
}