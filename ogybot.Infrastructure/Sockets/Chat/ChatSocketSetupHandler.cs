using Discord;
using Discord.WebSocket;
using ogybot.Communication.Exceptions;
using ogybot.Domain.Infrastructure.Security;
using ogybot.Domain.Infrastructure.Sockets.ChatSocket;

namespace ogybot.Data.Sockets.Chat;

public class ChatSocketSetupHandler : IChatSocketSetupHandler
{
    private readonly SocketIOClient.SocketIO _socket;
    private readonly ITokenRequester _tokenRequester;

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
}