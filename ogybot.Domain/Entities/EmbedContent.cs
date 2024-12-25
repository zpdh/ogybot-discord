using Discord.WebSocket;

namespace ogybot.Domain.Entities;

public class EmbedContent
{

    public SocketUser User { get; set; } = default!;
    public string? QueueSize { get; set; }
    public string? Description { get; set; }

    private EmbedContent(SocketUser user, string? queueSize, string? description)
    {
        User = user;
        QueueSize = queueSize;
        Description = description;
    }

    private EmbedContent()
    {
    }

    public static EmbedContent Create(SocketUser socketUser, string? queueSize = null, string? description = null)
    {
        return new EmbedContent(socketUser, queueSize, description);
    }
}