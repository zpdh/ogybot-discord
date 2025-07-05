using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Domain.Infrastructure.Clients;

public interface IUserClient
{
    Task LinkUserAsync(Guid wynnGuildId, LinkUser user);
    Task BanUserAsync(DiscordUser user);
    Task UnbanUserAsync(DiscordUser user);
    Task MuteUserAsync(DiscordUser user);
    Task UnmuteUserAsync(DiscordUser user);
}