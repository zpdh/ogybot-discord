using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using ogybot.Entities.Exceptions;
using ogybot.Utility.Extensions;

namespace ogybot.Bot.Handlers;

public class DiscordAppHandler
{
    public static DiscordSocketClient SetupDiscordClient()
    {
        var botConfig = new DiscordSocketConfig
        {
            UseInteractionSnowflakeDate = false,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };

        var discordClient = new DiscordSocketClient(botConfig);

        return discordClient;
    }

    public async Task StartupBotAsync(DiscordSocketClient client, IConfiguration configuration)
    {
        var token = GetBotToken(configuration);

        await client.LoginAsync(TokenType.Bot, token);
    }

    private static string GetBotToken(IConfiguration configuration)
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