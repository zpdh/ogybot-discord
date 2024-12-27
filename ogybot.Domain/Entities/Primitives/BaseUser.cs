namespace ogybot.Domain.Entities.Primitives;

public abstract class BaseUser
{

    public BaseUser()
    {

    }

    public BaseUser(string username)
    {
        Username = username;
    }

    public string? Username { get; set; }
}