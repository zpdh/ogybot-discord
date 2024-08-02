using Microsoft.EntityFrameworkCore;
using test.Api.Entities;

namespace test.Api.Repositories;

public sealed class WaitlistRepository : UserRepository<UserWaitlist>
{
    public WaitlistRepository(DataContext dataContext) : base(dataContext)
    {
    }
    
    public async Task<UserWaitlist?> SelectSingleAsync(UserWaitlist entity)
    {
        var user = await DataContext.UsersInWaitlist.FirstOrDefaultAsync(user =>
            user.Username!.Equals(entity.Username));

        return user;
    }
}