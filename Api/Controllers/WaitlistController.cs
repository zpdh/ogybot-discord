using test.Api.Entities;
using test.Api.Repositories;

namespace test.Api.Controllers;

public class WaitlistController
{
    private readonly WaitlistRepository _waitlistRepository = new();
    private readonly UnitOfWork _unitOfWork = new();
    
    public async Task<List<UserWaitlist>> GetWaitlistAsync()
    {
        var userList = await _waitlistRepository.SelectAllAsync();
        return userList;
    }

    public async Task<Response> RemovePlayerAsync(UserWaitlist user)
    {
        var dbUser = await _waitlistRepository.SelectSingleAsync(user);

        if (dbUser is null) return new Response(user.Username!, false);

        _waitlistRepository.Delete(user);
        await _unitOfWork.CommitAsync();
        
        return new Response(user.Username!, true);
    }

    public async Task<Response> AddPlayerAsync(UserWaitlist user)
    {
        var dbUser = await _waitlistRepository.SelectSingleAsync(user);
        if (dbUser is not null) return new Response(user.Username!, false);

        _waitlistRepository.Delete(user);
        await _unitOfWork.CommitAsync();

        return new Response(user.Username!, true);
    }
}