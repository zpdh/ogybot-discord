using Discord.Interactions;
using ogybot.Communication.Constants;
using ogybot.Communication.Exceptions;

namespace ogybot.Bot.Handlers;

public interface IBotExceptionHandler
{
    Task HandleAsync(SocketInteractionContext context, Exception exception);
}

public class BotExceptionHandler : IBotExceptionHandler
{
    public async Task HandleAsync(SocketInteractionContext context, Exception exception)
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
        // this is ephemeral because the response to buttons is ephemeral based on this value, while usually it is based on the value passed
        // in to the defer async before this
        await context.Interaction.FollowupAsync(exception.Message, ephemeral: true);
    }

    private static async Task HandleUnknownExceptionAsync(SocketInteractionContext context, Exception exception)
    {
        // see above for ephemeral explaination
        await context.Interaction.FollowupAsync(ErrorMessages.UnknownError, ephemeral: true);
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