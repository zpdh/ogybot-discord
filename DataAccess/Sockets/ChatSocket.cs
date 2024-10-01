using Discord;
using Discord.WebSocket;

namespace ogybot.DataAccess.Sockets;

/// <summary>
/// Class responsible for the handling of websocket requests.
/// </summary>
public class ChatSocket
{
    private readonly SocketIOClient.SocketIO _socket;

    public ChatSocket(string websocketUrl)
    {
        _socket = new SocketIOClient.SocketIO(websocketUrl);
    }

    public async void Start(IMessageChannel channel)
    {
        _socket.On("wynnMessage",
            async response => {
                var text = response.GetValue<string>();

                if (!string.IsNullOrWhiteSpace(text))
                {
                    await FormatAndSendMessageAsync(channel, text);
                }
            });

        _socket.OnConnected += (_, _) => Console.WriteLine("Successfully connected to Websocket Server");

        await _socket.ConnectAsync();
    }

    public async Task EmitMessageAsync(SocketMessage message)
    {
        await _socket.EmitAsync("discordMessage",
            new
            {
                Author = message.Author.Username,
                Content = message.CleanContent
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

        // Small delay to prevent going over discord's rate limit
        await Task.Delay(100);
        await channel.SendMessageAsync(embed: embed);
    }
}