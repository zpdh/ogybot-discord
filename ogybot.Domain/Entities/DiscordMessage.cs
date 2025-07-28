namespace ogybot.Domain.Entities;

public sealed class DiscordMessage
{

    public string DiscordUsername { get; set; } = string.Empty;
    public string DiscordUuid { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid WynnGuildId { get; set; }

    public DiscordMessage(string discordUsername, ulong discordUuid, string content, Guid wynnGuildId)
    {
        DiscordUsername = discordUsername;
        DiscordUuid = discordUuid.ToString();
        Content = content;
        WynnGuildId = wynnGuildId;
    }

    private DiscordMessage()
    {
    }
}