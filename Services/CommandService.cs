using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using test.Commands;
using test.Commands.TomeList;
using test.Commands.Waitlist;

namespace test.Services;

public class CommandService
{
    private static DiscordSocketClient _socketClient = null!;

    private readonly ulong _guildId;
    private readonly Dictionary<string, Func<SocketSlashCommand, Task>> _botCommand;

    public CommandService(DiscordSocketClient socketClient, ulong guildId)
    {
        _socketClient = socketClient;
        _guildId = guildId;

        //Start
        _botCommand = new Dictionary<string, Func<SocketSlashCommand, Task>>
        {
            {
                "ogy-cmdlist",
                async command => await ExecuteGlobalCommand(
                    ListCommand.ExecuteCommandAsync(command))
            },
            {
                "zingus",
                async command => await ExecuteGlobalCommand(
                    ZingusCommand.ExecuteCommandAsync(command))
            },
            {
                "raid",
                async command => await ExecuteChannelCommand(command, 863553410813001759,
                    RaidPingCommand.ExecuteCommandAsync(command))
            },
            {
                "war-build-help",
                async command => await ExecuteChannelCommand(command, 1255011451056423013,
                    WarQuestionCommand.ExecuteCommandAsync(command))
            },
            {
                "chiefs",
                async command => await ExecuteChannelCommand(command, 863553410813001759,
                    PingChiefsCommand.ExecuteCommandAsync(command))
            },
            {
                "tomelist",
                async command => await ExecuteChannelCommand(command, 1125517737188409364,
                    TomeListCommand.ExecuteCommandAsync(command))
            },
            {
                "tomelist-add",
                async command => await ExecuteChannelCommand(command, 1125517737188409364,
                    TomeListAddCommand.ExecuteCommandAsync(command))
            },
            {
                "tomelist-remove",
                async command => await ExecuteRemoveCommand(command, 1125517737188409364,
                    TomeListRemoveCommand.ExecuteCommandAsync(command))
            },
            {
                "waitlist",
                async command => await ExecuteChannelCommand(command, 1135296640803147806,
                    WaitlistCommand.ExecuteCommandAsync(command))
            },
            {
                "waitlist-add",
                async command => await ExecuteChannelCommand(command, 1135296640803147806,
                    WaitlistAddCommand.ExecuteCommandAsync(command))
            },
            {
                "waitlist-remove",
                async command => await ExecuteRemoveCommand(command, 1135296640803147806,
                    WaitlistRemoveCommand.ExecuteCommandAsync(command))
            }
        };
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

    private async Task InvalidChannelCommand(SocketSlashCommand command)
    {
        var userId = command.User.Id;
        await command.RespondAsync($"This is not a valid channel for this command , <@{userId}>!", ephemeral: true);
    }

    private async Task NoPermissionCommand(SocketSlashCommand command)
    {
        await command.RespondAsync("You do not have permissions to use this command.", ephemeral: true);
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
    }

    private async Task ExecuteGlobalCommand(Task method)
    {
        await method;
    }

    private async Task ExecuteChannelCommand(SocketSlashCommand command, ulong validChannelId, Task method)
    {
        /*
         * Tries to get channel id using the dictionary instanced at the constructor.
         * Compares with the channel the command was instanced in,
         * if it fails, simply returns invalid channel command.
         * Zingus command is an edge case, therefore it is not treated here.
         */

        if (command.ChannelId == validChannelId)
        {
            await method;
        }
        else
        {
            await InvalidChannelCommand(command);
        }
    }

    private async Task ExecuteRemoveCommand(SocketSlashCommand command, ulong validChannelId, Task method)
    {
        /*
         * Similar to last method, but also has to validate
         * user roles for the remove command.
         */

        var user = command.User as SocketGuildUser;
        var userRoles = user!.Roles.Where(r =>
            r.Id is 1060001967868485692
                or 810680884193787974
                or 1097935496442810419);

        if (!userRoles.Any())
        {
            await NoPermissionCommand(command);
            return;
        }

        await ExecuteChannelCommand(command, validChannelId, method);
    }
}