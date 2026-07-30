using ogybot.Communication.Constants;
using ogybot.Domain.Entities.Configurations;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Domain.Infrastructure.Security;

namespace ogybot.Data.Clients;

public sealed class GuildClient : BaseClient, IGuildClient
{
    private const string Endpoint = Endpoints.CONFIG;

    private readonly ITokenRequester _tokenRequester;

    public GuildClient(HttpClient httpClient, ITokenRequester tokenRequester) : base(httpClient)
    {
        _tokenRequester = tokenRequester;
    }

    public async Task<ServerConfiguration?> FetchConfigurationAsync(ulong discordGuildId)
    {
        var method = HttpMethod.Get;
        var token = await _tokenRequester.GetTokenAsync();

        var response = await MakeAndSendRequestAsync(method, $"{Endpoint}/{discordGuildId}", token: token);

        var serverConfiguration = await ParseResponseAsync<ServerConfiguration>(response);

        return serverConfiguration;
    }

    public async Task<ServerConfiguration?> MuteUserAsync(ulong discordGuildId, DiscordUser discordUser)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();

        var response = await MakeAndSendRequestAsync(method, $"{Endpoint}/{discordGuildId}/mute", new { DiscordUuid = discordUser.DiscordUuid.ToString() }, token: token);

        var serverConfiguration = await ParseResponseAsync<ServerConfiguration>(response);

        return serverConfiguration;
    }

    public async Task<ServerConfiguration?> UnmuteUserAsync(ulong discordGuildId, DiscordUser discordUser)
    {
        var method = HttpMethod.Delete;
        var token = await _tokenRequester.GetTokenAsync();

        var response = await MakeAndSendRequestAsync(method, $"{Endpoint}/{discordGuildId}/mute", new { DiscordUuid = discordUser.DiscordUuid.ToString() }, token: token);

        var serverConfiguration = await ParseResponseAsync<ServerConfiguration>(response);

        return serverConfiguration;
    }
}