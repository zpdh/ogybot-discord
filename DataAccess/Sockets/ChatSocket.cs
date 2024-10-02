using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using ogybot.DataAccess.Entities;
using ogybot.DataAccess.Enum;
using ogybot.DataAccess.Services;

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

    public async Task Start(IMessageChannel channel)
    {
        _socket.On("wynnMessage",
            async response => {
                var socketResponse = response.GetValue<SocketResponse>();

                if (!string.IsNullOrWhiteSpace(socketResponse.TextContent))
                {
                    await FormatAndSendMessageAsync(channel, socketResponse);
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

    private static async Task FormatAndSendMessageAsync(IMessageChannel channel, SocketResponse response)
    {
        var formattedMessage = response.TextContent;
        var embedBuilder = new EmbedBuilder();

        // Add extra embed options based on the selected message type
        switch (response.MessageType)
        {
            case SocketMessageType.ChatMessage:
                embedBuilder
                    .WithColor(Color.Teal);

                formattedMessage = $"**{response.HeaderContent}:** {response.TextContent}";

                break;

            case SocketMessageType.GuildMessage:
                embedBuilder
                    .WithAuthor(response.HeaderContent)
                    .WithColor(Color.DarkTeal);

                break;

            default:
                return;
        }

        var cleanedString = WhitespaceRemovalService.RemoveExcessWhitespaces(formattedMessage);

        embedBuilder.WithDescription(cleanedString);

        var embed = embedBuilder.Build();

        // Small delay to prevent going over discord's rate limit
        await Task.Delay(100);
        await channel.SendMessageAsync(embed: embed);
    }
}