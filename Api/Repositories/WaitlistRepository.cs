using Microsoft.EntityFrameworkCore;

namespace test.Api.Repositories;

public sealed class WaitlistRepository : UserRepository<UserWaitlist>
{
    public async Task<UserWaitlist?> SelectSingleAsync(UserWaitlist entity)
    {
        var user = await DataContext.UsersWaitlists.FirstOrDefaultAsync(user =>
            user.Username!.Equals(entity.Username));

        return user;
    }
}