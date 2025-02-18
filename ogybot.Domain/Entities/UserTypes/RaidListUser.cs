using ogybot.Domain.Primitives;

namespace ogybot.Domain.Entities.UserTypes;

public sealed class RaidListUser : User
{

    public double Aspects { get; set; }
    public double Emeralds { get; set; }
    public int Raids { get; set; }

    public RaidListUser(string username) : base(username)
    {

    }

    private RaidListUser()
    {

    }
}