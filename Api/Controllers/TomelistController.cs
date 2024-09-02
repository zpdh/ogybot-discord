using test.Api.Entities;
using test.Api.Repositories;
using test.Services;

namespace test.Api.Controllers;

/// <summary>
/// Class responsible for handling tome-related command requests
/// </summary>
public class TomelistController
{
    private readonly TomeClient _client = new();

    public async Task<List<UserTomelist>> GetTomelistAsync()
    {
        var userList = await _client.GetListAsync();
        return userList;
    }

    public async Task<Response> RemovePlayerAsync(UserTomelist user)
    {
        var result = await _client.RemoveUserAsync(user);
        return result;
    }

    public async Task<Response> AddPlayerAsync(UserTomelist user)
    {
        var result = await _client.PostUserAsync(user);
        return result;
    }
}