using ogybot.Domain.Primitives;

namespace ogybot.Domain.Entities.UserTypes;

public sealed class DiscordUser : User
{
    public DiscordUser(ulong discordUuid) : base("", discordUuid)
    {

    }

    private DiscordUser()
    {

    }
}