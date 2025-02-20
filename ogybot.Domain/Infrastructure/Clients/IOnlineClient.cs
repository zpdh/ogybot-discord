using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Domain.Infrastructure.Clients;

public interface IOnlineClient
{
    Task<IList<OnlineUser>> GetListAsync(Guid wynnGuildId);
}