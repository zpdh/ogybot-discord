using Discord;
using Discord.WebSocket;
using ogybot.Communication.Exceptions;
using ogybot.Domain.Services;

namespace ogybot.Data.Services;

public class DiscordChannelService : IDiscordChannelService
{
    private readonly DiscordSocketClient _client;

    public DiscordChannelService(DiscordSocketClient client)
    {
        _client = client;
    }


    public async Task<IMessageChannel> GetByIdAsync(ulong id)
    {
        if (await _client.GetChannelAsync(id) is not IMessageChannel channel)
        {
            throw new ChannelFetchingException();
        }

        return channel;
    }
}