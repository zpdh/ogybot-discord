using test.Api.Entities;
using test.Api.Repositories;
using test.Services;

namespace test.Api.Controllers;

public class TomelistController
{
    private readonly TomeRepository _tomeRepository;
    private readonly UnitOfWork _unitOfWork;

    public TomelistController()
    {
        var context = new DataContext();

        _tomeRepository = new TomeRepository(context);
        _unitOfWork = new UnitOfWork(context);
    }

    public async Task<List<UserTomelist>> GetTomelistAsync()
    {
        var userList = await _tomeRepository.SelectAllAsync();
        return userList;
    }

    public async Task<Response> RemovePlayerAsync(UserTomelist user)
    {
        var dbUser = await _tomeRepository.SelectSingleAsync(user);

        if (dbUser is null) return new Response(user.Username!, false);

        _tomeRepository.Delete(dbUser);
        await _unitOfWork.CommitAsync();
        
        return new Response(user.Username!, true);
    }

    public async Task<Response> AddPlayerAsync(UserTomelist user)
    {
        var dbUser = await _tomeRepository.SelectSingleAsync(user);
        if (dbUser is not null) return new Response(user.Username!, false);

         await _tomeRepository.InsertAsync(user);
         await _unitOfWork.CommitAsync();

        return new Response(user.Username!, true);
    }
}