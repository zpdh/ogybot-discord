namespace ogybot.Domain.DataTransferObjects;

public class RaidListUserDto
{
    public string Username { get; set; } = string.Empty;
    public double Aspects { get; set; }
    public double Emeralds { get; set; }

    public RaidListUserDto(string username, double aspects, double liquidEmeraldAmount)
    {
        Username = username;
        Aspects = aspects;
        Emeralds = liquidEmeraldAmount * 4096;
    }

    private RaidListUserDto()
    {

    }
}