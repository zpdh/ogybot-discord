using Newtonsoft.Json;
using ogybot.DataAccess.Entities;
using ogybot.DataAccess.Security;

namespace ogybot.DataAccess.Clients;

/// <summary>
/// Class responsible for handling requests and responses from external API
/// </summary>
public class OnlinePlayersClient
{
    private const string Endpoint = "discord/online";

    private readonly HttpClient _client;

    public OnlinePlayersClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<OnlineUser>> GetListAsync()
    {
        var listJson = await _client.GetAsync(Endpoint);

        var listOfUsers = JsonConvert.DeserializeObject<List<OnlineUser>>(
            await listJson.Content.ReadAsStringAsync());

        return listOfUsers!;
    }
}