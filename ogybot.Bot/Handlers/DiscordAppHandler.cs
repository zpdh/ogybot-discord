using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using ogybot.Entities.Exceptions;
using ogybot.Utility.Extensions;

namespace ogybot.Bot.Handlers;

public interface IDiscordAppHandler
{
    Task<DiscordSocketClient> SetupAndStartClientAsync();
}

public class DiscordAppHandler(IConfiguration configuration) : IDiscordAppHandler
{
    public async Task<DiscordSocketClient> SetupAndStartClientAsync()
    {
        var client = SetupDiscordClient();
        await StartupBotAsync(client);

        return client;
    }

    private static DiscordSocketClient SetupDiscordClient()
    {
        var botConfig = new DiscordSocketConfig
        {
            UseInteractionSnowflakeDate = false,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };

        var discordClient = new DiscordSocketClient(botConfig);

        return discordClient;
    }

    private async Task StartupBotAsync(DiscordSocketClient client)
    {
        var token = GetBotToken();

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();
    }

    private string GetBotToken()
    {
        var branch = configuration.GetValue<string>("Branch");
        var token = configuration.GetValue<string>($"Tokens:{branch}");

        if (token.IsNullOrWhitespace())
        {
            throw new InvalidBotTokenException();
        }

        return token;
    }
}