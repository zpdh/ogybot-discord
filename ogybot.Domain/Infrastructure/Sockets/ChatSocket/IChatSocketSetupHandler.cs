using Discord;
using Discord.WebSocket;

namespace ogybot.Domain.Infrastructure.Sockets.ChatSocket;

public interface IChatSocketSetupHandler
{
    Task StartAsync();
    Task RequestAndAddTokenToHeadersAsync();
    Task RequestAndRefreshTokenInHeadersAsync();
}