using System.Net.WebSockets;
using test.Builders;

namespace test;

public static class Program
{
    public static async Task Main()
    {
        var config = AppConfigurationBuilder.Build();
        var discordClient = await DiscordAppBuilder.SetupDiscordClientAsync(config);

        discordClient.AddCommands(config);


        // Keeps the bot running. If WebSocketException is thrown
        // (discord disconnects bot) simply logs on the console and
        // reconnects automatically.
        // If any exceptional exception is thrown, console log it for
        // debugging.
        while (true)
        {
            try
            {
                await Task.Delay(-1);
            }
            catch (Exception e)
            {
                Console.WriteLine("An exception occurred: " + e.Message);
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }
}