using ogybot.DataAccess.Enum;
using SocketIO.Core;

namespace ogybot.DataAccess.Entities;

public class SocketResponse
{
    public SocketMessageType MessageType { get; set; }
    public string HeaderContent { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
}