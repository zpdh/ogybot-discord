namespace ogybot.Domain.Entities.Primitives;

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