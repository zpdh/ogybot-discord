using Microsoft.Extensions.Configuration;
using test.Builders;
using test.Services;

namespace test;

public static class Program
{
    public static async Task Main()
    {
        var config = AppConfigurationBuilder.Build();
        var discordClient = await DiscordAppBuilder.SetupDiscordClientAsync(config);
        
        discordClient.AddCommands(config);

        await Task.Delay(-1);
    }
}