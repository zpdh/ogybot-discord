using ogybot.Domain.Entities;
using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Domain.Clients;

public interface IOnlineClient
{
    Task<IList<OnlineUser>> GetListAsync();
}