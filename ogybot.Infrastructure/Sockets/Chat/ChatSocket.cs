using Discord.WebSocket;
using ogybot.Domain.Infrastructure.Sockets.ChatSocket;

namespace ogybot.Data.Sockets.Chat;

public class ChatSocket : IChatSocket
{
    private readonly IChatSocketCommunicationHandler _communicationHandler;
    private readonly IChatSocketSetupHandler _setupHandler;

    public ChatSocket(IChatSocketSetupHandler setupHandler, IChatSocketCommunicationHandler communicationHandler)
    {
        _setupHandler = setupHandler;
        _communicationHandler = communicationHandler;
    }

    public async Task SetupAndStartAsync(DiscordSocketClient client)
    {
        await SetupClientAsync();
        _communicationHandler.SetupEmitter(client);
        await _setupHandler.StartAsync();
    }

    private async Task SetupClientAsync()
    {
        await _setupHandler.RequestAndAddTokenToHeadersAsync();
        _communicationHandler.SetupEventListeners();
    }
}