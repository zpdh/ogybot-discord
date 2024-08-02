using Discord;
using Discord.WebSocket;
using test.Services;

namespace test.Login;

public class Client
{
    private static readonly Dictionary<string, string> BotBranch = new Dictionary<string, string>
    {
        {
            "main",
            "MTI1MjQ2MzAyODAyNTQyNjAzMQ.GgbYj2.ItzY4IqZxVmZ2ol3BLoPZkuhifWlj-S1u9oR0k"
        },
        {
            "test",
            "MTI1NTUwNDYyNTQyMzg3NjE3OA.G4Jg5o.K7e54OtKLTmGcKEa0E7mt4VZz5N36Cel42cSQw"
        }
    };

    private readonly string _token = BotBranch["test"];

    public DiscordSocketClient client {  get; set; }

    public Client(DiscordSocketClient client)
    {
        this.client = client;
    }

    public async Task Login()
    {
        await client.LoginAsync(TokenType.Bot, _token);
        await client.StartAsync();
    }

    public void Log()
    {
        client.Log += LoggerService.Log;
    }

    public DiscordSocketClient GetClient()
    {
        return client;
    }
}