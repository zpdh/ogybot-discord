using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace ogybot.DataAccess.Sockets;

public class ChatServer
{
    private readonly HttpListener _listener;
    private readonly List<WebSocket> _sockets;

    public ChatServer(HttpListener listener, List<WebSocket> sockets)
    {
        _listener = listener;
        _sockets = sockets;

        _listener.Prefixes.Add("http://localhost:8080/");
    }

    private async Task HandleConnectionAsync(WebSocket webSocket)
    {
        var buffer = new byte[4096];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Closing connection",
                    CancellationToken.None);

                _sockets.Remove(webSocket);

                continue;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            if (result.MessageType == WebSocketMessageType.Text && !string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine(message);
            }
        }
    }

    public async Task StartServerAsync()
    {
        _listener.Start();
        Console.WriteLine("WebSocket server started successfully.");

        var context = await _listener.GetContextAsync();

        // Considerations:
        // This way will only accept a single web socket at a time.
        // In the future, if needed, simply nesting it inside a while statement should work.
        if (context.Request.IsWebSocketRequest)
        {
            var webSocketContext = await context.AcceptWebSocketAsync(null);
            var webSocket = webSocketContext.WebSocket;

            _sockets.Add(webSocket);

            await HandleConnectionAsync(webSocket);

            Console.WriteLine("New websocket connected");
        }

        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.Close();
        }
    }
}