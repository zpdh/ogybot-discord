using Discord.WebSocket;
using ogybot.Domain.Accessors;
using ogybot.Domain.Entities;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Domain.Infrastructure.Sockets.ChatSocket;
using ogybot.Domain.Services;
using ogybot.Utility.Extensions;
using ogybot.Utility.Services;

namespace ogybot.Data.Sockets.Chat;

public class ChatSocketCommunicationHandler : IChatSocketCommunicationHandler
{
    private readonly IDiscordChannelService _discordChannelService;
    private readonly IServerConfigurationAccessor _configurationAccessor;
    private readonly IChatSocketMessageHandler _messageHandler;
    private readonly IChatSocketSetupHandler _setupHandler;
    private readonly SocketIOClient.SocketIO _socket;

    public ChatSocketCommunicationHandler(
        IChatSocketMessageHandler messageHandler,
        SocketIOClient.SocketIO socket,
        IChatSocketSetupHandler setupHandler,
        IDiscordChannelService discordChannelService,
        IServerConfigurationAccessor configurationAccessor)
    {
        _messageHandler = messageHandler;
        _socket = socket;
        _setupHandler = setupHandler;
        _discordChannelService = discordChannelService;
        _configurationAccessor = configurationAccessor;
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
        var wynnGuildId = await GetWynnGuildIdAsync(message);

        if (MessageIsReply(message))
        {
            authorField = _messageHandler.AddReplyAuthorToField(message, authorField);
        }

        await _socket.EmitAsync("discordMessage",
            new DiscordMessage(authorField, cleanedContent, wynnGuildId));
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
        var broadcastingChannelId = await GetBroadcastingChannelIdAsync(message);

        if (message.Channel.Id != broadcastingChannelId) return;
        if (message.Author.IsBot || message is not SocketUserMessage userMessage) return;

        await EmitMessageAsync(userMessage);
    }

    private async Task<ulong> GetBroadcastingChannelIdAsync(SocketMessage message)
    {
        var discordGuildId = GetDiscordGuildId(message);

        var serverConfig = await _configurationAccessor.FetchServerConfigurationAsync(discordGuildId);
        return serverConfig!.BroadcastingChannel;
    }

    private static ulong GetDiscordGuildId(SocketMessage message)
    {
        return ((SocketGuildChannel)message.Channel).Guild.Id;
    }

    private async Task<Guid> GetWynnGuildIdAsync(SocketMessage message)
    {
        var discordGuildId = GetDiscordGuildId(message);
        var serverConfig = await _configurationAccessor.FetchServerConfigurationAsync(discordGuildId);

        return serverConfig.WynnGuildId;
    }

}