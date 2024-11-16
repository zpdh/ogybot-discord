using Discord;
using Discord.WebSocket;

namespace ogybot.Domain.Sockets;

public interface IChatSocket
{
    Task SetupAndStartAsync(DiscordSocketClient client, ulong channelId);
}