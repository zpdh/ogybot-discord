namespace ogybot.Domain.Primitives;

public abstract class User
{

    public string McUsername { get; set; } = string.Empty;
    public ulong DiscordUuid { get; set; } = 0;

    public User(string username, ulong discordUuid = 0)
    {
        McUsername = username;
        DiscordUuid = discordUuid;
    }

    protected User()
    {

    }
}