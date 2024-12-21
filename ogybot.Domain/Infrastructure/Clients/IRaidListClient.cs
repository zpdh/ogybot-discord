using ogybot.Domain.Entities;

namespace ogybot.Domain.Infrastructure.Clients;

public interface IRaidListClient
{
    Task<IList<RaidListUser>> GetListAsync();
    Task DecrementAspectAsync(RaidListUser user);
}