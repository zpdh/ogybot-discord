using System.Threading.Channels;
using ogybot.Communication.Constants;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Domain.Infrastructure.Security;

namespace ogybot.Data.Clients;

public class UserClient : BaseClient, IUserClient
{
    private const string Endpoint = Endpoints.USER;

    private readonly ITokenRequester _tokenRequester;

    public UserClient(HttpClient httpClient, ITokenRequester tokenRequester) : base(httpClient)
    {
        _tokenRequester = tokenRequester;
    }

    public async Task LinkUserAsync(Guid wynnGuildId, LinkUser user)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, $"{Endpoint}/link/{wynnGuildId}", new { user.McUsername, DiscordUuid = user.DiscordUuid.ToString() }, token);
    }
    public async Task BanUserAsync(DiscordUser user)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, $"{Endpoint}/ban/{user.DiscordUuid}", new { Banned = true}, token: token);
    }

    public async Task UnbanUserAsync(DiscordUser user)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, $"{Endpoint}/ban/{user.DiscordUuid}", new { Banned = false}, token: token);
    }
}