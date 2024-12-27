using ogybot.Domain.Entities.Configurations;

namespace ogybot.Domain.Accessors;

public interface IServerConfigurationAccessor
{
    Task<ServerConfiguration> FetchServerConfigurationAsync(ulong discordGuildId);
}