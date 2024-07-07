using Microsoft.EntityFrameworkCore;
using test.Services;

namespace test.Api;

public class TomeController
{
    private DataContext _dataContext = new DataContext();

    public async Task<IEnumerable<User>> GetTomeQueueAsync()
    {
        var userList = await _dataContext.Users.AsNoTracking().ToListAsync();
        return userList;
    }

    public async Task<string> RemovePlayerAsync(User user)
    {
        var dbUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName.ToUpper() == user.UserName.ToUpper());
        if (dbUser == null) return "User not found";
        
        _dataContext.Users.Remove(dbUser);
        await _dataContext.SaveChangesAsync();
        
        return $"Successfully removed player '{user.UserName}' from tome list";
    }

    public async Task<string> AddPlayerAsync(User user)
    {
        var dbUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName.ToUpper() == user.UserName.ToUpper());
        if (dbUser != null) return "User is already in list";
        
        await _dataContext.Users.AddAsync(user);
        await _dataContext.SaveChangesAsync();
        
        return $"Successfully added player '{user.UserName}' to tome list";
    }
}