using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Base;
using ogybot.Communication.Constants;
using ogybot.Communication.Exceptions;
using ogybot.Domain.Sockets;

namespace ogybot.Bot.Commands.Socket;

public class ChatSocketCommands : BasePermissionRequiredCommand
{
    private readonly IChatSocket _chatSocket;

    public ChatSocketCommands(IChatSocket chatSocket)
    {
        _chatSocket = chatSocket;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("startup-chat-socket", "Starts listening to socket messages in the channel this command is ran in.")]
    public async Task ExecuteStartupCommandAsync()
    {
        if (await IsInvalidContextAsync(GuildChannels.WebsocketLogChannel))
        {
            return;
        }

        await TryStartingSocketAsync();
    }

    private async Task TryStartingSocketAsync()
    {
        try
        {
            await ConfigureAndStartupListenerAsync();

            await FollowupAsync("Successfully started listening!");
        }
        catch (Exception)
        {
            throw new WebsocketStartupFailureException();
        }
    }

    private async Task ConfigureAndStartupListenerAsync()
    {

        await _chatSocket.SetupClientAsync(Context.Channel);
        await _chatSocket.StartAsync();
    }
}