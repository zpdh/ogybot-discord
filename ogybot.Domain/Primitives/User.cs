namespace ogybot.Domain.Primitives;

public abstract class User
{

    public string McUsername { get; set; } = string.Empty;
    public ulong DiscordUuid { get; set; } = 0;

    public User(string mcUsername, ulong discordUuid = 0)
    {
        McUsername = mcUsername;
        DiscordUuid = discordUuid;
    }

    protected User()
    {

    }
}