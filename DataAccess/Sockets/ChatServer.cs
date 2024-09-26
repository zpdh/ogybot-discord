using System.Net;
using System.Net.WebSockets;
using System.Text;
using Discord;
using Discord.WebSocket;

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

    private async Task HandleConnectionAsync(
        WebSocket webSocket,
        IMessageChannel channel)
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
                webSocket.Dispose();
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            if (result.MessageType == WebSocketMessageType.Text && !string.IsNullOrWhiteSpace(message))
            {
                // Small delay to ensure the bot won't get banned in case somebody spams too many requests
                await Task.Delay(100);
                await channel.SendMessageAsync(message);
            }
        }
    }

    public async Task StartServerAsync(IMessageChannel channel)
    {
        _listener.Start();
        Console.WriteLine("WebSocket server started successfully.");

        while (true)
        {
            var context = await _listener.GetContextAsync();

            if (context.Request.IsWebSocketRequest)
            {
                var webSocketContext = await context.AcceptWebSocketAsync(null);
                var webSocket = webSocketContext.WebSocket;

                _sockets.Add(webSocket);

                // Makes the method run in background to not block other requests
                _ = HandleConnectionAsync(webSocket, channel);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Close();
            }
        }
    }
}