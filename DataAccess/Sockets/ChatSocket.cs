using System.Net.Sockets;
using Discord;
using SocketIOClient;

namespace ogybot.DataAccess.Sockets;

/// <summary>
/// Class responsible for the handling of websocket requests.
/// </summary>
public class ChatSocket
{
    private readonly SocketIOClient.SocketIO _socket;

    public ChatSocket(string url)
    {
       _socket = new SocketIOClient.SocketIO(url);
    }

    public async void Start(IMessageChannel channel)
    {
        _socket.On("testmessage", async response => {
            string text = response.GetValue<string>();
            if (text != null) {
                await FormatAndSendMessageAsync(channel, text);
            }
        });
        Console.WriteLine("connecting");
        _socket.OnConnected += async (s, e) => {
            Console.WriteLine("socket io server connected");
            await _socket.EmitAsync("test", "hi");
        };
       await _socket.ConnectAsync();
    }

    private static async Task FormatAndSendMessageAsync(IMessageChannel channel, string message)
    {
        var formattedMessage = message;

        if (message.Contains(':'))
        {
            // Making the username bold
            var parts = message.Split(':', 2);
            formattedMessage = $"**{parts[0]}:**{parts[1]}";
        }

        var embedBuilder = new EmbedBuilder();
        embedBuilder
            .WithDescription(formattedMessage)
            .WithColor(Color.Teal);

        var embed = embedBuilder.Build();

        await channel.SendMessageAsync(embed: embed);
    }
}