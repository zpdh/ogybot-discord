using ogybot.Builders;

namespace ogybot;

public static class Program
{
    public static async Task Main()
    {
        // Instance and setup IConfiguration and DiscordSocketClient
        var config = AppConfigurationBuilder.Build();
        var discordClient = await DiscordAppBuilder.SetupDiscordClientAsync(config);

        discordClient.AddCommands(config);

        // Delay the task until program is closed
        await Task.Delay(-1);
    }
}