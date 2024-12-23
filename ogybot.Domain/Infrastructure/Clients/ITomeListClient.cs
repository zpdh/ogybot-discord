using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Domain.Infrastructure.Clients;

public interface ITomeListClient
{
    Task<IList<TomeListUser>> GetListAsync(Guid wynnGuildId);
    Task AddUserAsync(Guid wynnGuildId, TomeListUser user);
    Task RemoveUserAsync(Guid wynnGuildId, TomeListUser user);
}