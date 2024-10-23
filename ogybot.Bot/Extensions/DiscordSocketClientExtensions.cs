using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ogybot.Communication.Constants;
using ogybot.Communication.Exceptions;

namespace ogybot.Bot.Extensions;

public static class DiscordSocketClientExtensions
{
    public static void AddLogger(this DiscordSocketClient client)
    {
        client.Log += logMessage => {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        };
    }

    public static void AddInteraction(this DiscordSocketClient client, IServiceProvider services)
    {
        client.InteractionCreated += async interaction => {
            await interaction.DeferAsync();

            var interactionService = services.GetRequiredService<InteractionService>();

            var context = new SocketInteractionContext(client, interaction);
            var result = await interactionService.ExecuteCommandAsync(context, services);

            if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(ErrorMessages.UnknownError);
                throw new UnknownException(result.ErrorReason);
            }
        };
    }
}