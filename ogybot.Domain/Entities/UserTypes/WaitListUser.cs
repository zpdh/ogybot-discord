using ogybot.Domain.Primitives;

namespace ogybot.Domain.Entities.UserTypes;

public sealed class WaitListUser : User
{
    public WaitListUser(string mcUsername) : base(mcUsername)
    {

    }

    private WaitListUser()
    {

    }
}