using test.Api.Entities;
using test.Api.Repositories;
using test.Services;

namespace test.Api.Controllers;

public class TomelistController
{
    private readonly TomeRepository _tomeRepository = new();
    private readonly UnitOfWork _unitOfWork = new();

    public async Task<List<UserTomelist>> GetTomeQueueAsync()
    {
        var userList = await _tomeRepository.SelectAllAsync();
        return userList;
    }

    public async Task<Response> RemovePlayerAsync(UserTomelist user)
    {
        var dbUser = await _tomeRepository.SelectSingleAsync(user);

        if (dbUser is null) return new Response(user.Username!, false);

        _tomeRepository.Delete(user);
        await _unitOfWork.CommitAsync();
        
        return new Response(user.Username!, true);
    }

    public async Task<Response> AddPlayerAsync(UserTomelist user)
    {
        var dbUser = await _tomeRepository.SelectSingleAsync(user);
        if (dbUser is not null) return new Response(user.Username!, false);

        _tomeRepository.Delete(user);
        await _unitOfWork.CommitAsync();

        return new Response(user.Username!, true);
    }
}