using System.Net;
using System.Net.WebSockets;

namespace ogybot.DataAccess.Sockets;

public class ChatSocket
{
    private readonly HttpListener _listener;
    private readonly IList<WebSocket> _sockets;

    public ChatSocket(HttpListener listener, List<WebSocket> sockets)
    {
        _listener = listener;
        _sockets = sockets;

        _listener.Prefixes.Add("websocket link");
    }

    public async Task StartServerAsync()
    {
        _listener.Start();
        Console.WriteLine("WebSocket server started successfully.");

        while (true)
        {
            var context = await _listener.GetContextAsync();

            if (context.Request.IsWebSocketRequest)
            {
                var webSocketContext = await context.AcceptWebSocketAsync(null);
                _sockets.Add(webSocketContext.WebSocket);

                Console.WriteLine("New websocket connected");
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Close();
            }
        }
    }


}