using test.Api.Entities;
using test.Api.Repositories;

namespace test.Api.Controllers;

/// <summary>
/// Class responsible for handling waitlist-related command requests
/// </summary>
public class WaitlistController
{
    private readonly WaitlistClient _client = new();

    public async Task<List<UserWaitlist>> GetWaitlistAsync()
    {
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