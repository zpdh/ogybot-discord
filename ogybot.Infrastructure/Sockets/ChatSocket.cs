using Discord;
using Discord.WebSocket;
using ogybot.Communication.Exceptions;
using ogybot.Domain.Entities;
using ogybot.Domain.Security;
using ogybot.Utility.Extensions;
using ogybot.Utility.Services;
using SocketIOClient;
using SocketMessage = ogybot.Communication.Messages.SocketMessage;

namespace ogybot.Data.Sockets;

public class ChatSocket
{
    private const int DelayBetweenMessages = 250;

    private readonly SocketIOClient.SocketIO _socket;
    private readonly ITokenRequester _tokenRequester;

    public ChatSocket(ITokenRequester tokenRequester, string websocketUrl)
    {
        _tokenRequester = tokenRequester;
        _socket = ConfigureWebsocket(websocketUrl);
    }

    private static SocketIOClient.SocketIO ConfigureWebsocket(string websocketUrl)
    {
        return new SocketIOClient.SocketIO(websocketUrl,
            new SocketIOOptions
            {
                ExtraHeaders = new Dictionary<string, string>(),
                ConnectionTimeout = TimeSpan.FromSeconds(120),
                Reconnection = false
            });
    }

    public async Task StartAsync()
    {
        await _socket.ConnectAsync();
    }

    public async Task SetupClientAsync(IMessageChannel channel)
    {
        await RequestAndAddTokenToHeadersAsync();
        SetupEventListeners(channel);
    }

    private async Task RequestAndAddTokenToHeadersAsync()
    {
        var token = await _tokenRequester.GetTokenAsync();

        _socket.Options.ExtraHeaders.Add("Authorization", "Bearer " + token);
    }

    private void SetupEventListeners(IMessageChannel channel)
    {

        #region Websocket Events

        _socket.On("wynnMessage",
            async response => {
                var socketResponse = response.GetValue<SocketMessage>();

                if (!socketResponse.TextContent.IsNullOrWhitespace())
                {
                    await FormatAndSendEmbedAsync(channel, socketResponse);
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

    private static async Task FormatAndSendEmbedAsync(IMessageChannel channel, SocketMessage socketResponse)
    {

        var messageEmbed = FormatMessageIntoEmbed(socketResponse);
        await SendEmbedAsync(channel, messageEmbed);
    }

    private static Embed FormatMessageIntoEmbed(SocketMessage message)
    {
        var embedBuilder = new EmbedBuilder();
        var cleanedString = CleanUpResponseString(message, embedBuilder);

        var embed = CreateEmbed(embedBuilder, cleanedString);

        return embed;
    }

    private static Embed CreateEmbed(EmbedBuilder embedBuilder, string cleanedString)
    {

        embedBuilder.WithDescription(cleanedString);

        var embed = embedBuilder.Build();
        return embed;
    }

    private static string CleanUpResponseString(SocketMessage message, EmbedBuilder embedBuilder)
    {

        var formattedMessage = message.TextContent;

        // Can return null if message type is guild message
        var newFormattedMessage = FormatEmbedAccordingToMessageType(message, embedBuilder);
        formattedMessage = newFormattedMessage ?? formattedMessage;

        var cleanedString = WhitespaceRemovalService.RemoveExcessWhitespaces(formattedMessage);

        return cleanedString;
    }

    private static string? FormatEmbedAccordingToMessageType(SocketMessage response, EmbedBuilder embedBuilder)
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
                    .WithAuthor("Discord Only")
                    .WithColor(Color.Purple);

                message = $"**{response.HeaderContent}:** {response.TextContent}";
                break;

            case SocketMessageType.GuildMessage:
                embedBuilder
                    .WithAuthor(response.HeaderContent)
                    .WithColor(Color.Teal);

                break;

            default:
                throw new InvalidSocketArgumentException();
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
        var logEmbed = CreateLogEmbed(message);

        await channel.SendMessageAsync(embed: logEmbed);
    }

    private static Embed CreateLogEmbed(string message)
    {
        var messageAsEmbed = new EmbedBuilder()
            .WithColor(Color.Teal)
            .WithTitle("Websocket Log")
            .WithDescription(message)
            .Build();

        return messageAsEmbed;
    }

    public async Task EmitMessageAsync(SocketUserMessage message)
    {
        var authorField = message.Author.Username;
        var cleanedContent = WhitespaceRemovalService.RemoveExcessWhitespaces(message.CleanContent).Trim();

        if (MessageIsReply(message))
        {
            authorField = AddReplyAuthorToField(message, authorField);
        }

        await _socket.EmitAsync("discordMessage",
            new DiscordMessage(authorField, cleanedContent));
    }

    private static string AddReplyAuthorToField(SocketUserMessage message, string author)
    {
        var replyAuthor = message.ReferencedMessage.Author;
        author += $" (Replying to {replyAuthor})";
        return author;
    }

    private static bool MessageIsReply(SocketUserMessage message)
    {
        return message.ReferencedMessage != null;
    }
}