using ogybot.Domain.Entities.Configurations;

namespace ogybot.Domain.Clients;

public interface IGuildClient
{
    Task<ServerConfiguration> FetchConfigurationAsync(ulong discordGuildId);
}