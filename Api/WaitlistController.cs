using Microsoft.EntityFrameworkCore;

namespace test.Api;

public class WaitlistController
{
    private DataContext _dataContext = new DataContext();

    public async Task<IEnumerable<UserWaitlist>> GetWaitlistAsync()
    {
        var userList = await _dataContext.UserWaitlists.AsNoTracking().ToListAsync();
        return userList;
    }

    public async Task<string> RemovePlayerAsync(UserWaitlist user)
    {
        var dbUser =
            await _dataContext.UserWaitlists.FirstOrDefaultAsync(u => u.UserName.ToUpper() == user.UserName.ToUpper());
        if (dbUser == null) return "User not found";

        _dataContext.UserWaitlists.Remove(dbUser);
        await _dataContext.SaveChangesAsync();

        return $"Successfully removed player '{user.UserName}' from waitlist";
    }

    public async Task<string> AddPlayerAsync(UserWaitlist user)
    {
        var dbUser =
            await _dataContext.UserWaitlists.FirstOrDefaultAsync(u => u.UserName.ToUpper() == user.UserName.ToUpper());
        if (dbUser != null) return "User is already in list";

        await _dataContext.UserWaitlists.AddAsync(user);
        await _dataContext.SaveChangesAsync();

        return $"Successfully added player '{user.UserName}' to waitlist";
    }
}