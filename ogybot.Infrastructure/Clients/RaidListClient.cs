using ogybot.Domain.Clients;
using ogybot.Domain.Entities;
using ogybot.Domain.Security;

namespace ogybot.Data.Clients;

public class RaidListClient : BaseClient, IRaidListClient
{
    private const string Endpoint = "aspects";

    private readonly ITokenRequester _tokenRequester;

    public RaidListClient(HttpClient httpClient, ITokenRequester tokenRequester) : base(httpClient)
    {
        _tokenRequester = tokenRequester;
    }

    public async Task<IList<RaidListUser>> GetListAsync()
    {
        var method = HttpMethod.Get;

        var response = await MakeAndSendRequestAsync(method, Endpoint);

        var listOfUsers = await ParseResponseAsync<IList<RaidListUser>>(response);

        return listOfUsers;
    }

    public async Task DecrementAspectAsync(RaidListUser user)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();

        await MakeAndSendRequestAsync(method, Endpoint, user, token);
    }
}