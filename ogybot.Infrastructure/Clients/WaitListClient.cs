using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Domain.Infrastructure.Security;

namespace ogybot.Data.Clients;

public class WaitListClient : BaseClient, IWaitListClient
{
    private const string Endpoint = "guilds/waitlist";

    private readonly ITokenRequester _tokenRequester;

    public WaitListClient(HttpClient httpClient, ITokenRequester tokenRequester) : base(httpClient)
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

        await MakeAndSendRequestAsync(method, $"{Endpoint}/{wynnGuildId}/{user.Username}", token);
    }
}