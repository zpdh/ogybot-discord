using Discord;
using Discord.WebSocket;

namespace ogybot.Domain.Services;

public interface IDiscordChannelService
{
    Task<IMessageChannel> GetByIdAsync(ulong id);
}