using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using test.Commands;
using test.Commands.TomeList;
using test.Commands.Waitlist;
using test.Services;

namespace test.Builders;

public static class DiscordAppBuilder
{
    public static async Task<DiscordSocketClient> SetupDiscordClientAsync(IConfiguration configuration)
    {
        var config = new DiscordSocketConfig
        {
            UseInteractionSnowflakeDate = false
        };
        
        var discordClient = new DiscordSocketClient(config);

        await discordClient.ConnectAsync(configuration);

        discordClient.Log += LoggerService.Log;

        return discordClient;
    }

    public static void AddCommands(this DiscordSocketClient discordClient, IConfiguration configuration)
    {
        var branch = configuration["Branch"];
        var id = configuration.GetValue<ulong>($"ServerIds{branch}");

        CommandService commands = new(discordClient, id, AddCommandDictionary());

        //discordClient.Ready += commands.Client_Ready;
        discordClient.SlashCommandExecuted += commands.SlashCommandHandler;
    }

    private static async Task ConnectAsync(this DiscordSocketClient client, IConfiguration configuration)
    {
        var branch = configuration["Branch"];
        var token = configuration[$"Tokens:{branch}"];

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();
    }

    private static Dictionary<string, Func<SocketSlashCommand, Task>> AddCommandDictionary()
    {
        return new Dictionary<string, Func<SocketSlashCommand, Task>>
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
    
    private static async Task ExecuteGlobalCommand(Task method)
    {
        await method;
    }
    
    private static async Task ExecuteRemoveCommand(SocketSlashCommand command, ulong validChannelId, Task method)
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
    
    private static async Task ExecuteChannelCommand(SocketSlashCommand command, ulong validChannelId, Task method)
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
    
    private static async Task InvalidChannelCommand(SocketSlashCommand command)
    {
        var userId = command.User.Id;
        await command.RespondAsync($"This is not a valid channel for this command , <@{userId}>!", ephemeral: true);
    }

    private static async Task NoPermissionCommand(SocketSlashCommand command)
    {
        await command.RespondAsync("You do not have permissions to use this command.", ephemeral: true);
    }
}