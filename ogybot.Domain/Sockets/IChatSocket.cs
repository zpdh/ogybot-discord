using Discord;

namespace ogybot.Domain.Sockets;

public interface IChatSocket
{
    Task SetupClientAsync(IMessageChannel channel);
    Task StartAsync();
}