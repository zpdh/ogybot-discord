using ogybot.Domain.Entities.Primitives;

namespace ogybot.Domain.Entities.UserTypes;

public sealed class WaitListUser : User
{
    public WaitListUser(string username) : base(username)
    {

    }

    private WaitListUser()
    {

    }
}