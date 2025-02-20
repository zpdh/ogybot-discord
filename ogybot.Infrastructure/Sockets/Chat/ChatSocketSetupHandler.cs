using System.Net.WebSockets;
using ogybot.Domain.Infrastructure.Security;
using ogybot.Domain.Infrastructure.Sockets.ChatSocket;

namespace ogybot.Data.Sockets.Chat;

public class ChatSocketSetupHandler : IChatSocketSetupHandler
{
    private readonly SocketIOClient.SocketIO _socket;
    private readonly ITokenRequester _tokenRequester;

    private const int ReconnectionTries = 5;

    public ChatSocketSetupHandler(ITokenRequester tokenRequester, SocketIOClient.SocketIO socket)
    {
        _tokenRequester = tokenRequester;
        _socket = socket;
    }

    public async Task StartAsync()
    {
        await _socket.ConnectAsync();
    }

    public async Task RequestAndAddTokenToHeadersAsync()
    {
        var token = await _tokenRequester.GetTokenAsync();
        _socket.Options.ExtraHeaders.Add("Authorization", "Bearer " + token);
    }

    public async Task RequestAndRefreshTokenInHeadersAsync()
    {
        _socket.Options.ExtraHeaders.Remove("Authorization");
        await RequestAndAddTokenToHeadersAsync();
    }

    public async Task TryReconnectingAsync()
    {
        for (var i = 0; i < ReconnectionTries; i++)
        {
            try
            {
                await _socket.ConnectAsync();
            }
            catch (WebSocketException e)
            {
                // If reconnection fails, try again after 60 seconds.
                await Task.Delay(1000 * 60);
            } finally
            {
                Console.WriteLine($"Reconnection fail. Retry: {i}.");
            }
        }
    }
}