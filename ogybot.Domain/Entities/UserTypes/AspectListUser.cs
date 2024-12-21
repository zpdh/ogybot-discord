using ogybot.Domain.Entities.Primitives;

namespace ogybot.Domain.Entities;

public class AspectListUser : User
{
    public double Aspects { get; set; }
    public int EmeraldsOwed { get; set; }
    public int Raids { get; set; }

    public AspectListUser(string username) : base(username)
    {

    }

    private AspectListUser()
    {

    }
}