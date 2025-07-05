namespace ogybot.Domain.Primitives;

public abstract class User
{

    public string McUsername { get; set; } = string.Empty;
    public string DiscordUuid { get; set; } = string.Empty;

    public User(string username, string discordUuid = "")
    {
        McUsername = username;
        DiscordUuid = discordUuid;
    }

    protected User()
    {

    }
}