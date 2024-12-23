using ogybot.Domain.Entities.Primitives;

namespace ogybot.Domain.Entities;

public class RaidListUser : User
{

    public RaidListUser(string username) : base(username)
    {

    }

    private RaidListUser()
    {

    }

    public double Aspects { get; set; }
    public int EmeraldsOwed { get; set; }
    public int Raids { get; set; }
}