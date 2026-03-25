namespace ogybot.Domain.DataTransferObjects;

public class RaidListUserDto
{
    public string McUsername { get; set; } = string.Empty;
    public double Aspects { get; set; }
    public double Emeralds { get; set; }

    public RaidListUserDto(string mcUsername, double aspects, double liquidEmeraldAmount)
    {
        McUsername = mcUsername;
        Aspects = aspects;
        Emeralds = liquidEmeraldAmount * 4096;
    }

    private RaidListUserDto()
    {

    }
}