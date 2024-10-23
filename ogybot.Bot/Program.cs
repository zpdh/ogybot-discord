using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ogybot.Bot.Builders;

namespace ogybot.Bot;

public class Program
{
    public static async Task Main(string[] args)
    {
        var services = ServiceBuilder.Build();
        var discordClient = services.GetRequiredService<DiscordSocketClient>();

        await Task.Delay(-1);
    }
}