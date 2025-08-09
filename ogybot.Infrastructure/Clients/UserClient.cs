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

        await MakeAndSendRequestAsync(method, $"{Endpoint}/link/{wynnGuildId}", user, token);
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

    public async Task MuteUserAsync(DiscordUser user)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, $"{Endpoint}/mute/{user.DiscordUuid}", new { Muted = true}, token: token);
    }

    public async Task UnmuteUserAsync(DiscordUser user)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, $"{Endpoint}/mute/{user.DiscordUuid}", new { Muted = false}, token: token);
    }
}