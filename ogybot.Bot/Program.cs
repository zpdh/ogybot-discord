using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ogybot.Bot.Builders;
using ogybot.Bot.Handlers;

namespace ogybot.Bot;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var services = ServiceBuilder.Build();

        var discordClientHandler = services.GetRequiredService<IDiscordAppHandler>();
        var discordClient = services.GetRequiredService<DiscordSocketClient>();

        await Task.Delay(-1);
    }
}