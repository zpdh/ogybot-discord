using ogybot.Communication.Requests;
using ogybot.Domain.DataTransferObjects;
using ogybot.Domain.Entities;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;
using ogybot.Domain.Infrastructure.Security;

namespace ogybot.Data.Clients;

public class RaidListClient : BaseClient, IRaidListClient
{
    private const string Endpoint = "guilds/raids/rewards";

    private readonly ITokenRequester _tokenRequester;

    public RaidListClient(HttpClient httpClient, ITokenRequester tokenRequester) : base(httpClient)
    {
        _tokenRequester = tokenRequester;
    }

    public async Task<IList<RaidListUser>> GetListAsync(Guid wynnGuildId)
    {
        var method = HttpMethod.Get;

        var response = await MakeAndSendRequestAsync(method, $"{Endpoint}/{wynnGuildId}");

        var listOfUsers = await ParseResponseAsync<IList<RaidListUser>>(response);

        return listOfUsers;
    }

    public async Task DecrementRewardsAsync(Guid wynnGuildId, RaidListUserDto user)
    {
        var method = HttpMethod.Post;
        var token = await _tokenRequester.GetTokenAsync();
        var request = new DecrementRewardsRequest(user.Username, user.Aspects, user.Emeralds);

        await MakeAndSendRequestAsync(method, $"{Endpoint}/{wynnGuildId}", request, token);
    }
}