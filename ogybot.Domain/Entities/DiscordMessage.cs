namespace ogybot.Domain.Entities;

public sealed class DiscordMessage
{

    public string Author { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid WynnGuildId { get; set; }

    public DiscordMessage(string author, string content, Guid wynnGuildId)
    {
        Author = author;
        Content = content;
        WynnGuildId = wynnGuildId;
    }

    private DiscordMessage()
    {
    }
}