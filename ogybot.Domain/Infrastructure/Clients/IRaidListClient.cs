using ogybot.Domain.Entities;

namespace ogybot.Domain.Infrastructure.Clients;

public interface IRaidListClient
{
    Task<IList<RaidListUser>> GetListAsync(Guid wynnGuildId);
    Task DecrementAspectAsync(Guid wynnGuildId, RaidListUser user);
}