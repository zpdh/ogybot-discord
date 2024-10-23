using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ogybot.Communication.Constants;
using ogybot.Communication.Exceptions;
using ogybot.Utility.Extensions;

namespace ogybot.Bot.Extensions;

public static class DiscordSocketClientExtensions
{
    public static void AddEvents(this DiscordSocketClient client, IServiceProvider services)
    {
        client.AddLogger();
        client.AddInteraction(services);
    }

    private static void AddLogger(this DiscordSocketClient client)
    {
        client.Log += logMessage => {
            Console.WriteLine(logMessage.Message);

            return Task.CompletedTask;
        };
    }

    private static void AddInteraction(this DiscordSocketClient client, IServiceProvider services)
    {
        client.InteractionCreated += async interaction => {
            await interaction.DeferAsync();

            await HandleCommandExecutionAsync(client, services, interaction);
        };
    }

    private static async Task HandleCommandExecutionAsync(DiscordSocketClient client, IServiceProvider services, SocketInteraction interaction)
    {

        var context = new SocketInteractionContext(client, interaction);
        var result = await TryExecuteCommandAsync(context, services);

        if (!result.IsSuccess)
        {
            await HandleCommandFailureAsync(context, result);
        }
    }

    private static async Task<IResult> TryExecuteCommandAsync(
        SocketInteractionContext context,
        IServiceProvider services)
    {
        var interactionService = services.GetRequiredService<InteractionService>();

        var result = await interactionService.ExecuteCommandAsync(context, services);

        return result;
    }

    private static async Task HandleCommandFailureAsync(SocketInteractionContext context, IResult result)
    {

        await context.Channel.SendMessageAsync(ErrorMessages.UnknownError);
        throw new UnknownException(result.ErrorReason);
    }
}