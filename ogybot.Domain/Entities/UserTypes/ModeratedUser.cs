using ogybot.Domain.Primitives;

namespace ogybot.Domain.Entities.UserTypes;

public sealed class ModeratedUser : User
{
    public ModeratedUser(string discordUuid) : base("", discordUuid)
    {

    }

    private ModeratedUser()
    {

    }
}