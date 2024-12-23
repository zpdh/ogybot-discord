using ogybot.Domain.Entities.Configurations;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Domain.Infrastructure.Security;

namespace ogybot.Data.Clients;

public sealed class GuildClient : BaseClient, IGuildClient
{
    private const string Endpoint = "guilds/config";

    private readonly ITokenRequester _tokenRequester;

    public GuildClient(HttpClient httpClient, ITokenRequester tokenRequester) : base(httpClient)
    {
        _tokenRequester = tokenRequester;
    }

    public async Task<ServerConfiguration?> FetchConfigurationAsync(ulong discordGuildId)
    {
        var method = HttpMethod.Get;
        var token = await _tokenRequester.GetTokenAsync();

        var response = await MakeAndSendRequestAsync(method, $"{Endpoint}/{discordGuildId}", token);

        var serverConfiguration = await ParseResponseAsync<ServerConfiguration>(response);

        return serverConfiguration;
    }
}