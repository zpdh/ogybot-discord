using Discord.Interactions;
using Discord.WebSocket;
using ogybot.Communication.Exceptions;

namespace ogybot.Bot.Handlers;

public class ExceptionHandler
{
    public static async Task HandleAsync(SocketInteractionContext context, Exception exception)
    {
        if (exception is OgybotException)
        {
            await HandleOgybotExceptionAsync(context, exception);
        }
        else
        {
            await HandleUnknownExceptionAsync(context, exception);
        }
    }

    private static async Task HandleOgybotExceptionAsync(SocketInteractionContext context, Exception exception)
    {

        await context.Interaction.FollowupAsync(exception.Message);
    }

    private static async Task HandleUnknownExceptionAsync(SocketInteractionContext context, Exception exception)
    {

        await context.Interaction.FollowupAsync("An unhandled exception occurred. Contact a developer.");
        Log(exception);
    }

    private static void Log(Exception exception)
    {
        Console.WriteLine(
            "An exception occurred!" +
            $"Exception Message: {exception.Message}" +
            $"Stack Trace: {exception.StackTrace}");
    }
}