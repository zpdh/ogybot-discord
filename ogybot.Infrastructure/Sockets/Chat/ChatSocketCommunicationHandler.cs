using Discord;
using Discord.WebSocket;
using ogybot.Domain.Entities;
using ogybot.Domain.Entities.Configurations;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Domain.Infrastructure.Sockets.ChatSocket;
using ogybot.Domain.Services;
using ogybot.Utility.Extensions;
using ogybot.Utility.Services;

namespace ogybot.Data.Sockets.Chat;

public class ChatSocketCommunicationHandler : IChatSocketCommunicationHandler
{
    private readonly IChatSocketMessageHandler _messageHandler;
    private readonly IChatSocketSetupHandler _setupHandler;
    private readonly IDiscordChannelService _discordChannelService;
    private readonly IGuildClient _guildClient;
    private readonly SocketIOClient.SocketIO _socket;

    public ChatSocketCommunicationHandler(
        IChatSocketMessageHandler messageHandler,
        SocketIOClient.SocketIO socket,
        IChatSocketSetupHandler setupHandler,
        IDiscordChannelService discordChannelService,
        IGuildClient guildClient)
    {
        _messageHandler = messageHandler;
        _socket = socket;
        _setupHandler = setupHandler;
        _discordChannelService = discordChannelService;
        _guildClient = guildClient;
    }

    public void SetupEventListeners()
    {

        #region Websocket Events

        _socket.On("wynnMessage",
            async response => {
                var socketResponse = response.GetValue<ChatSocketMessage>();
                var channel = await _discordChannelService.GetByIdAsync(socketResponse.ListeningChannel);

                if (!socketResponse.TextContent.IsNullOrWhitespace())
                {
                    await _messageHandler.FormatAndSendEmbedAsync(channel, socketResponse);
                }
            });

        #endregion

        #region Websocket Connectivity Events

        _socket.OnConnected += (_, _) => {
            const string message = "Successfully connected to Websocket Server";

            Console.WriteLine(message);
        };

        _socket.OnDisconnected += async (_, reason) => {
            var message = $"Disconnected from Websocket Server. Reason: {reason}";

            Console.WriteLine(message);

            await _setupHandler.RequestAndRefreshTokenInHeadersAsync();

            await _socket.ConnectAsync();
        };

        #endregion

    }

    public async Task EmitMessageAsync(SocketUserMessage message)
    {
        var authorField = message.Author.Username;
        var cleanedContent = WhitespaceRemovalService.RemoveExcessWhitespaces(message.CleanContent).Trim();

        if (MessageIsReply(message))
        {
            authorField = _messageHandler.AddReplyAuthorToField(message, authorField);
        }

        await _socket.EmitAsync("discordMessage",
            new DiscordMessage(authorField, cleanedContent));
    }

    public void SetupEmitter(DiscordSocketClient client)
    {
        client.MessageReceived += SetupMessageReceiverAsync;
    }

    private static bool MessageIsReply(SocketUserMessage message)
    {
        return message.ReferencedMessage != null;
    }

    private async Task SetupMessageReceiverAsync(SocketMessage message)
    {
        var broadcastingChannelId = await GetBroadcastingChannelId(message);

        if (message.Channel.Id != broadcastingChannelId) return;
        if (message.Author.IsBot || message is not SocketUserMessage userMessage) return;

        await EmitMessageAsync(userMessage);
    }

    private async Task<ulong> GetBroadcastingChannelId(SocketMessage message)
    {

        var discordGuildId = ((SocketGuildChannel)message.Channel).Guild.Id;

        var serverConfig = await _guildClient.FetchConfigurationAsync(discordGuildId);
        return serverConfig.BroadcastingChannel;
    }
}