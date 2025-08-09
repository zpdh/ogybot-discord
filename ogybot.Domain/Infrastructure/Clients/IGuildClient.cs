using ogybot.Domain.Entities.Configurations;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Domain.Infrastructure.Clients;

public interface IGuildClient
{
    Task<ServerConfiguration?> FetchConfigurationAsync(ulong discordGuildId);
    Task<ServerConfiguration?> MuteUserAsync(ulong discordGuildId, DiscordUser discordUuid);
    Task<ServerConfiguration?> UnmuteUserAsync(ulong discordGuildId, DiscordUser discordUuid);
}