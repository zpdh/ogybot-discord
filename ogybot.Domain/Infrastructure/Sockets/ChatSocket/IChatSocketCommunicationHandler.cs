using Discord;
using Discord.WebSocket;

namespace ogybot.Domain.Infrastructure.Sockets.ChatSocket;

public interface IChatSocketCommunicationHandler
{
    void SetupEventListeners();
    Task EmitMessageAsync(SocketUserMessage message);
    void SetupEmitter(DiscordSocketClient client);
}