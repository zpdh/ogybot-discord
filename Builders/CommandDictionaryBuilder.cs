using Discord.WebSocket;
using test.Commands;
using test.Commands.AspectList;
using test.Commands.TomeList;
using test.Commands.Waitlist;

namespace test.Builders;

public static class CommandDictionaryBuilder
{
    public static Dictionary<string, Func<SocketSlashCommand, Task>> Build()
    {
        return new Dictionary<string, Func<SocketSlashCommand, Task>>
        {
            {
                "ogy-cmdlist",
                async command => await ExecuteGlobalCommand(
                    command, ListCommand.ExecuteCommandAsync)
            },
            {
                "zingus",
                async command => await ExecuteGlobalCommand(
                    command, ZingusCommand.ExecuteCommandAsync)
            },
            {
                "raid",
                async command => await ExecuteChannelCommand(command, 863553410813001759,
                    RaidPingCommand.ExecuteCommandAsync)
            },
            {
                "war-build-help",
                async command => await ExecuteChannelCommand(command, 1255011451056423013,
                    WarQuestionCommand.ExecuteCommandAsync)
            },
            {
                "chiefs",
                async command => await ExecuteChannelCommand(command, 863553410813001759,
                    PingChiefsCommand.ExecuteCommandAsync)
            },
            {
                "tomelist",
                async command => await ExecuteChannelCommand(command, 1125517737188409364,
                    TomeListCommand.ExecuteCommandAsync)
            },
            {
                "tomelist-add",
                async command => await ExecuteChannelCommand(command, 1125517737188409364,
                    TomeListAddCommand.ExecuteCommandAsync)
            },
            {
                "tomelist-remove",
                async command => await ExecuteRemoveCommand(command, 1125517737188409364,
                    TomeListRemoveCommand.ExecuteCommandAsync)
            },
            {
                "waitlist",
                async command => await ExecuteChannelCommand(command, 1135296640803147806,
                    WaitlistCommand.ExecuteCommandAsync)
            },
            {
                "waitlist-add",
                async command => await ExecuteChannelCommand(command, 1135296640803147806,
                    WaitlistAddCommand.ExecuteCommandAsync)
            },
            {
                "waitlist-remove",
                async command => await ExecuteRemoveCommand(command, 1135296640803147806,
                    WaitlistRemoveCommand.ExecuteCommandAsync)
            },
            {
                "aspectlist",
                async command => await ExecuteChannelCommand(command, 1272044811771449365,
                    AspectListCommand.ExecuteCommandAsync)
            },
            {
                "aspectlist-decrement",
                async command => await ExecuteRemoveCommand(command, 1272044811771449365,
                    DecrementAspectCommand.ExecuteCommandAsync)
            }
        };
    }

    private static async Task ExecuteGlobalCommand(SocketSlashCommand command, Func<SocketSlashCommand, Task> method)
    {
        await command.DeferAsync();
        await method(command);
    }

    private static async Task ExecuteRemoveCommand(SocketSlashCommand command, ulong validChannelId,
        Func<SocketSlashCommand, Task> method)
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

    private static async Task ExecuteChannelCommand(SocketSlashCommand command, ulong validChannelId,
        Func<SocketSlashCommand, Task> method)
    {
        /*
         * Tries to get channel id using the dictionary instanced at the constructor.
         * Compares with the channel the command was instanced in,
         * if it fails, simply returns invalid channel command.
         */

        await command.DeferAsync();

        if (command.ChannelId == validChannelId)
        {
            await method(command);
        }
        else
        {
            await InvalidChannelCommand(command);
        }
    }

    private static async Task InvalidChannelCommand(SocketSlashCommand command)
    {
        var userId = command.User.Id;
        await command.FollowupAsync($"This is not a valid channel for this command , <@{userId}>!", ephemeral: true);
    }

    private static async Task NoPermissionCommand(SocketSlashCommand command)
    {
        await command.FollowupAsync("You do not have permissions to use this command.", ephemeral: true);
    }
}