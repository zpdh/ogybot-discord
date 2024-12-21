using ogybot.Domain.Entities.Configurations;

namespace ogybot.Domain.Infrastructure.Clients;

public interface IGuildClient
{
    Task<ServerConfiguration> FetchConfigurationAsync(ulong discordGuildId);
}