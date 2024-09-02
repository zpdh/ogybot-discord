using Discord.WebSocket;

namespace test.Commands;

internal interface ICommand
{
    // STATIC ABSTRACT METHODS LETS GOOOOOOOO
    static abstract Task ExecuteCommandAsync(SocketSlashCommand command);
    static abstract Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId);
}