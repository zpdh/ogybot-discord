using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.SlashCommands.IndividualCommands
{
    internal interface ICommand
    {
        static Task GenerateCommand() => throw new NotImplementedException();

        static Task ExecuteCommandAsync(SocketSlashCommand command) => throw new NotImplementedException();
    }
}
