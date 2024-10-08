using System.Net.Http.Json;
using System.Text;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ogybot.DataAccess.Entities;
using ogybot.DataAccess.Enum;
using ogybot.DataAccess.Services;

namespace ogybot.DataAccess.Sockets;

/// <summary>
/// Class responsible for the handling of websocket requests.
/// </summary>
public class ChatSocket
{
    private readonly HttpClient _client;
    private readonly string _validationKey;
    private readonly SocketIOClient.SocketIO _socket;

    private const int DelayBetweenMessages = 250;
    private const string DiscordMessageAuthor = "Discord Only";

    public ChatSocket(IConfiguration configuration)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(configuration["Api:Uri"]!)
        };

        _validationKey = configuration["Api:ValidationKey"]!;
        _socket = new SocketIOClient.SocketIO(configuration["Websocket:WebsocketServerUrl"], new SocketIOClient.SocketIOOptions{ExtraHeaders= new Dictionary<string, string>{}});
    }

    public async Task StartAsync(IMessageChannel channel)
    {
        var token = await GetTokenAsync();
        
        _socket.Options.ExtraHeaders.Add("Authorization", "bearer " + token);

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
        if (message is not SocketUserMessage userMessage) return;

        var cleanedString = WhitespaceRemovalService.RemoveExcessWhitespaces(userMessage.CleanContent).Trim();

        // Checks if message is reply, if it is, concat the author of the reply in the message
        if (userMessage.ReferencedMessage is not null)
        {
            var referencedMessageAuthor = userMessage.ReferencedMessage.Author;
            cleanedString += $"(replying to {referencedMessageAuthor})";
        }

        await _socket.EmitAsync("discordMessage",
            new
            {
                Author = userMessage.Author.Username,
                Content = cleanedString
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
                    .WithColor(Color.Blue);

                formattedMessage = $"**{response.HeaderContent}:** {response.TextContent}";

                break;

            case SocketMessageType.DiscordMessage:
                embedBuilder
                    .WithAuthor(DiscordMessageAuthor)
                    .WithColor(Color.Purple);

                formattedMessage = $"**{response.HeaderContent}:** {response.TextContent}";

                break;

            case SocketMessageType.GuildMessage:
                embedBuilder
                    .WithAuthor(response.HeaderContent)
                    .WithColor(Color.Teal);

                break;

            default: return;
        }

        var cleanedString = WhitespaceRemovalService.RemoveExcessWhitespaces(formattedMessage);

        embedBuilder.WithDescription(cleanedString);

        var embed = embedBuilder.Build();

        // Small delay to prevent going over discord's rate limit
        await Task.Delay(DelayBetweenMessages);
        await channel.SendMessageAsync(embed: embed);
    }
    private async Task<string?> GetTokenAsync()
    {
        // Serialize validation key to JSON
        var json = JsonConvert.SerializeObject(new
        {
            validationKey = _validationKey
        });

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        // Get token response from API
        var response = await _client.PostAsync("auth/gettoken", content);

        if (!response.IsSuccessStatusCode) return null;

        var apiResponse = await response.Content.ReadFromJsonAsync<TokenApiResponse>();
        return apiResponse!.Token;
    }
}