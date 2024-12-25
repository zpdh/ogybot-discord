using ogybot.Communication.Constants;
using ogybot.Communication.Exceptions;
using ogybot.CrossCutting.Accessors.Abstractions;
using ogybot.Domain.Entities.Configurations;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Domain.Services;

namespace ogybot.CrossCutting.Accessors.Implementations;

public class ServerConfigurationAccessor : IServerConfigurationAccessor
{
    private readonly ICacheService _cacheService;
    private readonly IGuildClient _guildClient;

    public ServerConfigurationAccessor(IGuildClient guildClient, ICacheService cacheService)
    {
        _guildClient = guildClient;
        _cacheService = cacheService;
    }

    public async Task<ServerConfiguration> FetchServerConfigurationAsync(ulong discordGuildId)
    {
        var cacheConfiguration = FetchFromCache(discordGuildId);

        if (cacheConfiguration is not null) return cacheConfiguration;

        var apiConfiguration = await FetchFromApiAsync(discordGuildId);

        AddToCache(apiConfiguration);

        return apiConfiguration;
    }

    private ServerConfiguration? FetchFromCache(ulong discordGuildId)
    {
        return _cacheService.QueryFor<ServerConfiguration>($"ServerConfiguration:{discordGuildId}");
    }

    private void AddToCache(ServerConfiguration configuration)
    {
        _cacheService.InsertInto($"ServerConfiguration:{configuration.DiscordGuildId}", configuration);
    }

    private async Task<ServerConfiguration> FetchFromApiAsync(ulong discordGuildId)
    {
        var serverConfiguration = await _guildClient.FetchConfigurationAsync(discordGuildId);

        if (serverConfiguration is null)
        {
            throw new FetchingException(ExceptionMessages.GuildNotConfigured);
        }

        return serverConfiguration;
    }
}