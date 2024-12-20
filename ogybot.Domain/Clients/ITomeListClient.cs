using ogybot.Domain.Entities;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Domain.Clients;

public interface ITomeListClient
{
    Task<IList<TomeListUser>> GetListAsync();
    Task AddUserAsync(TomeListUser user);
    Task RemoveUserAsync(TomeListUser user);
}