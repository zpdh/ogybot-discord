using Discord.WebSocket;

namespace ogybot.Domain.Infrastructure.Sockets.ChatSocket;

public interface IChatSocket
{
    Task SetupAndStartAsync(DiscordSocketClient client, ulong channelId);
}