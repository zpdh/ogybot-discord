namespace ogybot.Domain.Entities.Primitives;

public abstract class User
{

    public User(string username)
    {
        Username = username;
    }

    protected User()
    {

    }

    public string Username { get; set; }
}