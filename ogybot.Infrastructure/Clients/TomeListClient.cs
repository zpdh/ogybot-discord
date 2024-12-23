using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Domain.Infrastructure.Security;

namespace ogybot.Data.Clients;

public class TomeListClient : BaseClient, ITomeListClient
{
    private const string Endpoint = "guilds/tomes";

    private readonly ITokenRequester _tokenRequester;

    public TomeListClient(HttpClient httpClient, ITokenRequester tokenRequester) : base(httpClient)
    {
        _tokenRequester = tokenRequester;
    }

    public async Task<IList<TomeListUser>> GetListAsync(Guid wynnGuildId)
    {
        var method = HttpMethod.Get;

        var response = await MakeAndSendRequestAsync(method, $"{Endpoint}/{wynnGuildId}");

        var listOfUsers = await ParseResponseAsync<IList<TomeListUser>>(response);

        return listOfUsers;
    }

    public async Task AddUserAsync(Guid wynnGuildId, TomeListUser user)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, $"{Endpoint}/{wynnGuildId}", user, token);
    }

    public async Task RemoveUserAsync(Guid wynnGuildId, TomeListUser user)
    {
        var method = HttpMethod.Delete;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, $"{Endpoint}/{wynnGuildId}/{user.Username}", token);
    }
}