using ogybot.Domain.DataTransferObjects;
using ogybot.Domain.Entities;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Domain.Infrastructure.Clients;

public interface IRaidListClient
{
    Task<IList<RaidListUser>> GetListAsync(Guid wynnGuildId);
    Task DecrementRewardsAsync(Guid wynnGuildId, RaidListUserDto raidListUser);
}