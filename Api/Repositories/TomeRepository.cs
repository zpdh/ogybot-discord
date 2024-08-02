using Microsoft.EntityFrameworkCore;
using test.Api.Entities;
using test.Services;

namespace test.Api.Repositories;

public sealed class TomeRepository : UserRepository<UserTomelist>
{
    public async Task<UserTomelist?> SelectSingleAsync(UserTomelist entity)
    {
        var user = await DataContext.UsersInTomelist.FirstOrDefaultAsync(user =>
            user.Username!.Equals(entity.Username));
        
        return user;
    }
}