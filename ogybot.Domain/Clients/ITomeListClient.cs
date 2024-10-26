using ogybot.Domain.Entities;

namespace ogybot.Domain.Clients;

public interface ITomeListClient
{
    Task<IList<TomeListBaseUser>> GetListAsync();
    Task AddUserAsync(TomeListBaseUser user);
    Task RemoveUserAsync(TomeListBaseUser user);
}