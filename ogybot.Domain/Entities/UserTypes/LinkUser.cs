using ogybot.Domain.Primitives;

namespace ogybot.Domain.Entities.UserTypes;

public sealed class LinkUser : User
{
    public LinkUser(string mcUsername, ulong discordUuid) : base(mcUsername, discordUuid)
    {

    }

    private LinkUser()
    {

    }
}