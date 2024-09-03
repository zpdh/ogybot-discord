using Discord;

namespace ogybot.Services;

/// <summary>
/// Class responsible for logging. Only command errors are logged on this application.
/// </summary>
public static class LoggerService
{
    public static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}