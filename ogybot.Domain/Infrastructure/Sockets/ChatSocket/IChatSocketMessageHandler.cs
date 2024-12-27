using Discord;
using Discord.WebSocket;
using ogybot.Domain.Entities;

namespace ogybot.Domain.Infrastructure.Sockets.ChatSocket;

public interface IChatSocketMessageHandler
{
    Task FormatAndSendEmbedAsync(IMessageChannel channel, ChatSocketMessage chatSocketResponse);
    Task SendLoggingMessageAsync(IMessageChannel channel, string message);
    string AddReplyAuthorToField(SocketUserMessage message, string author);
}