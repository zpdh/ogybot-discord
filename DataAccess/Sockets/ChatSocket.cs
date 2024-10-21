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

    public async Task StartAsync()
    {
        await _socket.ConnectAsync();
    }

    public async Task SetupClientAsync(IMessageChannel channel)
    {
        await AddTokenToHeadersAsync();
        SetupEventListeners(channel);
    }

    private async Task AddTokenToHeadersAsync()
    {
        var token = await _tokenGenerator.GetTokenAsync();

        _socket.Options.ExtraHeaders.Add("Authorization", "Bearer " + token);
    }

    private void SetupEventListeners(IMessageChannel channel)
    {

        #region Websocket Events

        _socket.On("wynnMessage",
            async response => {
                var socketResponse = response.GetValue<SocketResponse>();

                if (!string.IsNullOrWhiteSpace(socketResponse.TextContent))
                {
                    var messageEmbed = FormatMessageIntoEmbed(socketResponse);
                    await SendEmbedAsync(channel, messageEmbed);
                }
            });

        #endregion

        #region Websocket Connectivity Events

        _socket.OnConnected += async (_, _) => {
            const string message = "Successfully connected to Websocket Server";

            Console.WriteLine(message);
            await SendLoggingMessageAsync(channel, message);
        };

        _socket.OnDisconnected += async (_, reason) => {
            var message = $"Disconnected from Websocket Server. Reason: {reason}";

            Console.WriteLine(message);
            await SendLoggingMessageAsync(channel, message);

            await _socket.ConnectAsync();
        };

        _socket.OnReconnectFailed += async (_, _) => {
            const string message = "Could not reconnect to Websocket Server.";

            Console.WriteLine(message);
            await SendLoggingMessageAsync(channel, message);
        };

        #endregion

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

    private static Embed FormatMessageIntoEmbed(SocketResponse response)
    {
        var embedBuilder = new EmbedBuilder();
        var cleanedString = CleanUpResponseString(response, embedBuilder);

        embedBuilder.WithDescription(cleanedString);

        var embed = embedBuilder.Build();

        return embed;
    }

    private static string CleanUpResponseString(SocketResponse response, EmbedBuilder embedBuilder)
    {

        var formattedMessage = response.TextContent;

        // Add extra embed options based on the selected message type
        var newFormattedMessage = FormatEmbedAccordingToMessageType(response, embedBuilder);
        formattedMessage = newFormattedMessage ?? formattedMessage;

        var cleanedString = WhitespaceRemovalService.RemoveExcessWhitespaces(formattedMessage);

        return cleanedString;
    }

    private static string? FormatEmbedAccordingToMessageType(SocketResponse response, EmbedBuilder embedBuilder)
    {
        string? message = null;

        switch (response.MessageType)
        {
            case SocketMessageType.ChatMessage:
                embedBuilder
                    .WithColor(Color.Blue);

                message = $"**{response.HeaderContent}:** {response.TextContent}";
                break;

            case SocketMessageType.DiscordMessage:
                embedBuilder
                    .WithAuthor(DiscordMessageAuthor)
                    .WithColor(Color.Purple);

                message = $"**{response.HeaderContent}:** {response.TextContent}";
                break;

            case SocketMessageType.GuildMessage:
                embedBuilder
                    .WithAuthor(response.HeaderContent)
                    .WithColor(Color.Teal);
                break;

            default:
                break;
        }

        return message;
    }

    private static async Task SendEmbedAsync(IMessageChannel channel, Embed embed)
    {
        // Small delay to prevent going over discord's rate limit
        await Task.Delay(DelayBetweenMessages);
        await channel.SendMessageAsync(embed: embed);
    }

    private static async Task SendLoggingMessageAsync(IMessageChannel channel, string message)
    {
        var messageAsEmbed = new EmbedBuilder()
            .WithColor(Color.Teal)
            .WithTitle("Websocket Log")
            .WithDescription(message)
            .Build();

        await channel.SendMessageAsync(embed: messageAsEmbed);
    }
}