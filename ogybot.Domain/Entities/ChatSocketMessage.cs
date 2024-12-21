using ogybot.Domain.Enums;

namespace ogybot.Domain.Entities;

public sealed class ChatSocketMessage
{
    public SocketMessageType MessageType { get; set; }
    public string HeaderContent { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
}