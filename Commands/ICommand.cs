using Discord.WebSocket;

namespace test.Commands;

internal interface ICommand
{
    static Task GenerateCommand() => throw new NotImplementedException();

    static Task ExecuteCommandAsync(SocketSlashCommand command) => throw new NotImplementedException();
}