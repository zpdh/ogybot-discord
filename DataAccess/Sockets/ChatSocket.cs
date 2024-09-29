using System.Net;
using System.Net.WebSockets;
using System.Text;
using Discord;
using Discord.WebSocket;
using Quobject.SocketIoClientDotNet.Client;

namespace ogybot.DataAccess.Sockets;

/// <summary>
/// Class responsible for the handling of websocket requests.
/// </summary>
public class ChatSocket
{
    private readonly Socket _socket;

    public ChatSocket(string url)
    {
        _socket = IO.Socket(url);
    }

    public void Start(IMessageChannel channel)
    {
        _socket.On("connect",
            () => {
                Console.WriteLine("Successfully connected to Websocket Server.");
            });

        _socket.On("disconnect",
            () => {
                Console.WriteLine("Disconnected from Websocket Server");
            });

        _socket.On("message",
            (msg) => {
                if (msg is string msgAsString)
                {
                    _ = FormatAndSendMessageAsync(channel, msgAsString);
                }
            });
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