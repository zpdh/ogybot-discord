using Discord;

namespace ogybot.Domain.Services;

public interface IDiscordChannelService
{
    Task<IMessageChannel> GetByIdAsync(ulong id);
}