using Discord.WebSocket;
using test.Commands;
using test.Commands.TomeList;
using test.SlashCommands;

namespace test.Services
{
    public class CommandsControllerService
    {
        private static DiscordSocketClient _socketClient { get; set; }

        public CommandsControllerService(DiscordSocketClient socketClient)
        {
            _socketClient=socketClient;
        }

        public async Task Client_Ready()
        {
            //ulong guildId = 810258030201143328;
            // ^^^ MAIN SERVER

             ulong guildId = 1255321257956605963;
            // ^^^ TEST SERVER

            var guild = _socketClient.GetGuild(guildId);

            await ListCommand.GenerateCommandAsync(_socketClient, guildId);
            await RaidPingCommand.GenerateCommandAsync(_socketClient, guildId);
            await WarQuestionCommand.GenerateCommandAsync(_socketClient, guildId);
            await OutdatedCommands.GenerateCommandAsync(_socketClient, guildId);
            await PingChiefsCommand.GenerateCommandAsync(_socketClient, guildId);
            await TomeListCommand.GenerateCommandAsync(_socketClient, guildId);
            await TomeListAddCommand.GenerateCommandAsync(_socketClient, guildId);
            await TomeListRemoveCommand.GenerateCommandAsync(_socketClient, guildId);
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "raid":
                    if (command.ChannelId != 863553410813001759) await InvalidChannelCommand(command);
                    else await RaidPingCommand.ExecuteCommandAsync(command);
                    break;

                case "war-build-help":
                    if (command.ChannelId != 1255011451056423013) await InvalidChannelCommand(command);
                    else await WarQuestionCommand.ExecuteCommandAsync(command);
                    break;

                case "chiefs":
                    if (command.ChannelId != 863553410813001759) await InvalidChannelCommand(command);
                    await PingChiefsCommand.ExecuteCommandAsync(command);
                    break;

                case "ogy-cmdlist":
                    await ListCommand.ExecuteCommandAsync(command);
                    break;

                case "build-help":
                    await OutdatedCommands.ExecuteCommandAsync(command);
                    break;
                case "tomelist":
                    await TomeListCommand.ExecuteCommandAsync(command);
                    break;
                case "tomelist-add":
                    await TomeListAddCommand.ExecuteCommandAsync(command);
                    break;
                case "tomelist-remove":
                    await TomeListRemoveCommand.ExecuteCommandAsync(command);
                    break;
            }
        }

        public async Task InvalidChannelCommand(SocketSlashCommand command)
        {
            var userId = command.User.Id;
            await command.RespondAsync($"This is not a valid channel for this command , <@{userId}>!", ephemeral: true);
        }
    }
}
