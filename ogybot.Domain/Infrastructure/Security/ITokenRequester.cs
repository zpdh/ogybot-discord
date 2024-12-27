namespace ogybot.Domain.Infrastructure.Security;

public interface ITokenRequester
{
    Task<string> GetTokenAsync();
}