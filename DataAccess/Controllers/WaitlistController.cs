using ogybot.DataAccess.Clients;
using ogybot.DataAccess.Entities;

namespace ogybot.DataAccess.Controllers;

/// <summary>
/// Class responsible for handling waitlist-related command requests
/// </summary>
public class WaitlistController
{
    private readonly WaitlistClient _client;

    public WaitlistController(WaitlistClient client)
    {
        _client = client;
    }

    public async Task<List<UserWaitlist>> GetWaitlistAsync()
    {
        // The list doesn't need to be sorted as the API already does it for us.
        var list = await _client.GetListAsync();
        return list;
    }

    public async Task<Response> RemovePlayerAsync(UserWaitlist user)
    {
        var response = await _client.RemoveUserAsync(user);
        return response;
    }

    public async Task<Response> AddPlayerAsync(UserWaitlist user)
    {
        var response = await _client.PostUserAsync(user);
        return response;
    }
}