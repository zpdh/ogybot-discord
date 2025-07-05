using ogybot.Domain.Entities.UserTypes;

namespace ogybot.Domain.Infrastructure.Clients;

public interface IUserClient
{
    Task BanUserAsync(ModeratedUser user);
    Task UnbanUserAsync(ModeratedUser user);
    Task MuteUserAsync(ModeratedUser user);
    Task UnmuteUserAsync(ModeratedUser user);
}