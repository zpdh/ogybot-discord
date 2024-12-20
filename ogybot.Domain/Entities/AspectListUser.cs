namespace ogybot.Domain.Entities;

public class AspectListUser : BaseUser
{

    public AspectListUser()
    {

    }

    public AspectListUser(string username) : base(username)
    {

    }

    public double Aspects { get; set; }
    public int EmeraldsOwed { get; set; }
    public int Raids { get; set; }
}