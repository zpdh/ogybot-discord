using ogybot.Domain.Entities;

namespace ogybot.Communication.Messages;

public class SocketMessage
{
    public SocketMessageType MessageType { get; set; }
    public string HeaderContent { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
}