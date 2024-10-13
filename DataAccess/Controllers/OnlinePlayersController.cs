using ogybot.DataAccess.Clients;
using ogybot.DataAccess.Entities;

namespace ogybot.DataAccess.Controllers;

/// <summary>
/// Class responsible for handling the online command request
/// </summary>
public class OnlinePlayersController
{
    private readonly OnlinePlayersClient _client;

    public OnlinePlayersController(OnlinePlayersClient client)
    {
        _client = client;
    }

    public async Task<List<OnlineUser>> GetOnlinePlayersAsync()
    {
        var onlinePlayersList = await _client.GetListAsync();
        return onlinePlayersList;
    }
}