using Discord.Net;
using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Login;
using Discord.Rest;
using Discord.Commands;
using System.Reactive.Concurrency;
using test.SlashCommands.IndividualCommands;

namespace test.Commands
{
    public class CommandsController
    {
        private static DiscordSocketClient? _socketClient { get; set; }

        public CommandsController(DiscordSocketClient socketClient)
        {
            _socketClient=socketClient;
        }

        public async Task Client_Ready()
        {
            ulong guildId = 810258030201143328;
            // ^^^ MAIN SERVER

            //ulong guildId = 1255321257956605963;
            // ^^^ TEST SERVER

            var guild = _socketClient.GetGuild(guildId);

            await ListCommand.GenerateCommandAsync(_socketClient, guildId);
            await RaidPingCommand.GenerateCommandAsync(_socketClient, guildId);
            await WarQuestionCommand.GenerateCommandAsync(_socketClient, guildId);
            await OutdatedCommands.GenerateCommandAsync(_socketClient, guildId);
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

                case "ogy-cmdlist":
                    await ListCommand.ExecuteCommandAsync(command);
                    break;

                case "build-help":
                    await OutdatedCommands.ExecuteCommandAsync(command);
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
