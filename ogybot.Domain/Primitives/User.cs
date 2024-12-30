namespace ogybot.Domain.Primitives;

public abstract class User
{

    public string Username { get; set; } = string.Empty;

    public User(string username)
    {
        Username = username;
    }

    protected User()
    {

    }
}