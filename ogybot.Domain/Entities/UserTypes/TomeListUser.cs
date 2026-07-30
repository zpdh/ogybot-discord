using ogybot.Domain.Primitives;

namespace ogybot.Domain.Entities.UserTypes;

public sealed class TomeListUser : User
{
    public TomeListUser(string mcUsername) : base(mcUsername)
    {

    }

    private TomeListUser()
    {

    }
}