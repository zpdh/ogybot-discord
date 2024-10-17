using Discord;
using Discord.WebSocket;
using ogybot.DataAccess.Entities;
using ogybot.DataAccess.Enum;
using ogybot.DataAccess.Security;
using ogybot.DataAccess.Services;
using SocketIOClient;

namespace ogybot.DataAccess.Sockets;

/// <summary>
/// Class responsible for the handling of websocket requests.
/// </summary>
public class ChatSocket
{
    private readonly SocketIOClient.SocketIO _socket;
    private readonly TokenGenerator _tokenGenerator;

    private const int DelayBetweenMessages = 250;
    private const string DiscordMessageAuthor = "Discord Only";

    public ChatSocket(TokenGenerator tokenGenerator, string webSocketUrl)
    {
        _tokenGenerator = tokenGenerator;
        _socket = new SocketIOClient.SocketIO(webSocketUrl,
            new SocketIOOptions
            {
                // Need to initialize the ExtraHeaders dictionary, as the library doesn't do so
                ExtraHeaders = new Dictionary<string, string>(),
                // Increase the connection timeout as render can sometimes take a while to connect
                ConnectionTimeout = TimeSpan.FromSeconds(120),
            });
    }

    public async Task StartAsync(IMessageChannel channel)
    {
        var token = await _tokenGenerator.GetTokenAsync();

        _socket.Options.ExtraHeaders.Add("Authorization", "Bearer " + token);

        _socket.On("wynnMessage",
            async response => {
                var socketResponse = response.GetValue<SocketResponse>();

                if (!string.IsNullOrWhiteSpace(socketResponse.TextContent))
                {
                    var messageEmbed = FormatMessage(socketResponse);
                    await SendEmbedAsync(channel, messageEmbed);
                }
            });

        _socket.OnConnected += (_, _) =>
            Console.WriteLine("Successfully connected to Websocket Server");

        _socket.OnDisconnected += (_, reason) =>
            Console.WriteLine($"Disconnected from Websocket Server. Reason: {reason}");

        _socket.OnReconnectFailed += (_, _) =>
            Console.WriteLine("Could not reconnect to Websocket Server.");

        await _socket.ConnectAsync();
    }

    public async Task EmitMessageAsync(SocketUserMessage message)
    {
        var author = message.Author.Username;
        var cleanedContent = WhitespaceRemovalService.RemoveExcessWhitespaces(message.CleanContent).Trim();

        // Checks if message is reply, if it is, concat the author of the reply in the header content
        if (message.ReferencedMessage is not null)
        {
            var replyAuthor = message.ReferencedMessage.Author;
            author += $" (Replying to {replyAuthor})";
        }

        await _socket.EmitAsync("discordMessage",
            new DiscordMessage(author, cleanedContent));
    }

    private static Embed FormatMessage(SocketResponse response)
    {
        var formattedMessage = response.TextContent;
        var embedBuilder = new EmbedBuilder();

        // Add extra embed options based on the selected message type
        switch (response.MessageType)
        {
            case SocketMessageType.ChatMessage:
                embedBuilder
                    .WithColor(Color.Blue);

                formattedMessage = $"**{response.HeaderContent}:** {response.TextContent}";

                break;

            case SocketMessageType.DiscordMessage:
                embedBuilder
                    .WithAuthor(DiscordMessageAuthor)
                    .WithColor(Color.Purple);

                formattedMessage = $"**{response.HeaderContent}:** {response.TextContent}";

                break;

            case SocketMessageType.GuildMessage:
                embedBuilder
                    .WithAuthor(response.HeaderContent)
                    .WithColor(Color.Teal);

                break;

            // Need to change default case later.
            default:
                break;
        }

        var cleanedString = WhitespaceRemovalService.RemoveExcessWhitespaces(formattedMessage);

        embedBuilder.WithDescription(cleanedString);

        var embed = embedBuilder.Build();

        return embed;
    }

    private static async Task SendEmbedAsync(IMessageChannel channel, Embed embed)
    {
        // Small delay to prevent going over discord's rate limit
        await Task.Delay(DelayBetweenMessages);
        await channel.SendMessageAsync(embed: embed);
    }
}