using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Domain.Infrastructure.Clients;

public interface IWaitListClient
{
    Task<IList<WaitListUser>> GetListAsync(Guid wynnGuildId);
    Task AddUserAsync(Guid wynnGuildId, WaitListUser user);
    Task RemoveUserAsync(Guid wynnGuildId, WaitListUser user);
}