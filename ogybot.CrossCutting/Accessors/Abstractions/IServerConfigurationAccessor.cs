using ogybot.Domain.Entities.Configurations;

namespace ogybot.CrossCutting.Accessors.Abstractions;

public interface IServerConfigurationAccessor
{
    Task<ServerConfiguration> FetchServerConfigurationAsync(ulong discordGuildId);
}