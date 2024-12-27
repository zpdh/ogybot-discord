using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Data.Clients;

public class OnlineClient : BaseClient, IOnlineClient
{
    private const string Endpoint = "guilds/online";

    public OnlineClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<IList<OnlineUser>> GetListAsync(Guid wynnGuildId)
    {
        var method = HttpMethod.Get;

        var response = await MakeAndSendRequestAsync(method, $"{Endpoint}/{wynnGuildId}");

        var listOfUsers = await ParseResponseAsync<IList<OnlineUser>>(response);

        return listOfUsers;
    }
}