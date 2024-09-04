using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ogybot.Builders;

namespace ogybot.Extensions;

public static class ServiceExtensions
{
    public static void AddServices(this ServiceCollection services)
    {
        services.AddConfiguration();
        services.AddDiscordClient();
        services.AddInteractionService();
    }

    private static void AddConfiguration(this ServiceCollection services)
    {
        services.AddSingleton(AppConfigurationBuilder.Build());
    }

    private static void AddDiscordClient(this ServiceCollection services)
    {
        services.AddSingleton(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();

            var discordClient = DiscordAppBuilder.SetupDiscordClientAsync(config).GetAwaiter().GetResult();
            return discordClient;
        });
    }

    private static void AddInteractionService(this ServiceCollection services)
    {
        services.AddSingleton(provider =>
        {
            var client = provider.GetRequiredService<DiscordSocketClient>();

            return services.AddSingleton(new InteractionService(client.Rest));
        });
    }
}