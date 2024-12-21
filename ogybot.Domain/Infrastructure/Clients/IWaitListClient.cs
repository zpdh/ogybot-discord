using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Domain.Infrastructure.Clients;

public interface IWaitListClient
{
    Task<IList<WaitListUser>> GetListAsync();
    Task AddUserAsync(WaitListUser user);
    Task RemoveUserAsync(WaitListUser user);
}