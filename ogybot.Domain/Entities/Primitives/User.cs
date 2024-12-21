namespace ogybot.Domain.Entities.Primitives;

public abstract class User
{
    public string Username { get; set; }

    public User(string username)
    {
        Username = username;
    }

    protected User()
    {

    }
}