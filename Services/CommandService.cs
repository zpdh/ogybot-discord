using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using ogybot.Commands.AspectList;
using ogybot.Commands.Other;
using ogybot.Commands.TomeList;
using ogybot.Commands.Waitlist;
using ogybot.Commands;

namespace ogybot.Services;

/// <summary>
/// Class responsible for the management and instantiation of slash commands.
/// When a command gets executed, this class gives it instructions on
/// what to do next.
/// </summary>
public class CommandService
{
    private static DiscordSocketClient _socketClient = null!;

    private readonly ulong _guildId;
    private readonly Dictionary<string, Func<SocketSlashCommand, Task>> _botCommand;

    public CommandService(
        DiscordSocketClient socketClient,
        ulong guildId,
        Dictionary<string, Func<SocketSlashCommand, Task>> commandDictionary)
    {
        _socketClient = socketClient;
        _guildId = guildId;

        //Start
        _botCommand = commandDictionary;
    }

    public async Task Client_Ready()
    {
        await InstantiateCommands(_guildId);
    }

    public async Task SlashCommandHandler(SocketSlashCommand command)
    {
        var cmdName = command.Data.Name;
        await _botCommand[cmdName].Invoke(command);
    }

    private static async Task InstantiateCommands(ulong guildId)
    {
        await ListCommand.GenerateCommandAsync(_socketClient, guildId);
        await RaidPingCommand.GenerateCommandAsync(_socketClient, guildId);
        await WarQuestionCommand.GenerateCommandAsync(_socketClient, guildId);
        await OutdatedCommands.GenerateCommandAsync(_socketClient, guildId);
        await PingChiefsCommand.GenerateCommandAsync(_socketClient, guildId);
        await TomeListCommand.GenerateCommandAsync(_socketClient, guildId);
        await TomeListAddCommand.GenerateCommandAsync(_socketClient, guildId);
        await TomeListRemoveCommand.GenerateCommandAsync(_socketClient, guildId);
        await ZingusCommand.GenerateCommandAsync(_socketClient, guildId);
        await WaitlistCommand.GenerateCommandAsync(_socketClient, guildId);
        await WaitlistAddCommand.GenerateCommandAsync(_socketClient, guildId);
        await WaitlistRemoveCommand.GenerateCommandAsync(_socketClient, guildId);
        await AspectListCommand.GenerateCommandAsync(_socketClient, guildId);
        await DecrementAspectCommand.GenerateCommandAsync(_socketClient, guildId);
    }
}