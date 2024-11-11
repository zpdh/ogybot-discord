using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Base;
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
        await TryStartingSocketAsync();
    }

    private async Task TryStartingSocketAsync()
    {
        try
        {
            await _chatSocket.StartAsync();
            await _chatSocket.SetupClientAsync(Context.Channel);

            await FollowupAsync("Successfully started listening!");
        }
        catch (Exception e)
        {
            await FollowupAsync("Could not start listener up. Exception:" + e.Message);
        }
    }
}