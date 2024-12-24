namespace ogybot.Domain.DataTransferObjects;

public class RaidListUserDto
{
    public string Username { get; set; } = string.Empty;
    public double AspectAmount { get; set; }
    public double LiquidEmeraldAmount { get; set; }

    public RaidListUserDto(string username, double aspectAmount, double liquidEmeraldAmount)
    {
        Username = username;
        AspectAmount = aspectAmount;
        LiquidEmeraldAmount = liquidEmeraldAmount;
    }

    private RaidListUserDto()
    {

    }
}