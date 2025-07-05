using System.Threading.Channels;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Domain.Infrastructure.Security;

namespace ogybot.Data.Clients;

public class UserClient : BaseClient, IUserClient
{
    private const string Endpoint = "user";

    private readonly ITokenRequester _tokenRequester;

    public UserClient(HttpClient httpClient, ITokenRequester tokenRequester) : base(httpClient)
    {
        _tokenRequester = tokenRequester;
    }

    public async Task<IList<WaitListUser>> GetListAsync(Guid wynnGuildId)
    {
        var method = HttpMethod.Get;

        var response = await MakeAndSendRequestAsync(method, $"{Endpoint}/{wynnGuildId}");

        var listOfUsers = await ParseResponseAsync<IList<WaitListUser>>(response);

        return listOfUsers;
    }

    public async Task AddUserAsync(Guid wynnGuildId, WaitListUser user)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, $"{Endpoint}/{wynnGuildId}", user, token);
    }

    public async Task RemoveUserAsync(Guid wynnGuildId, WaitListUser user)
    {
        var method = HttpMethod.Delete;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, $"{Endpoint}/{wynnGuildId}/{user.McUsername}", token: token);
    }

    public async Task BanUserAsync(ModeratedUser user)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, $"{Endpoint}/ban/{user.DiscordUuid}", new { Banned = true}, token: token);
    }

    public async Task UnbanUserAsync(ModeratedUser user)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, $"{Endpoint}/ban/{user.DiscordUuid}", new { Banned = false}, token: token);
    }

    public async Task MuteUserAsync(ModeratedUser user)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, $"{Endpoint}/mute/{user.DiscordUuid}", new { Muted = true}, token: token);
    }

    public async Task UnmuteUserAsync(ModeratedUser user)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, $"{Endpoint}/mute/{user.DiscordUuid}", new { Muted = false}, token: token);
    }
}