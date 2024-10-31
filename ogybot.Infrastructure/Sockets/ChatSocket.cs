using Discord;
using ogybot.Domain.Security;
using SocketIOClient;

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

    public async Task RequestAndAddTokenToHeadersAsync()
    {
        var token = await _tokenRequester.GetTokenAsync();

        _socket.Options.ExtraHeaders.Add("Authorization", "Bearer " + token);
    }

    private void SetupEventListeners(IMessageChannel channel)
    {

    }
}