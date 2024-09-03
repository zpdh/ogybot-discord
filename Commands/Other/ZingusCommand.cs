using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace ogybot.Commands.Other;

public abstract class ZingusCommand : ICommand
{
         public static async Task ExecuteCommandAsync(SocketSlashCommand command)
        {
            await command.FollowupAsync("https://tenor.com/view/zingus-cat-kitty-caption-cat-walk-gif-19570879");
        }

        public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
        {
            try
            {
                var guildCommand = new SlashCommandBuilder()
                .WithName("zingus")
                .WithDescription("zingusposts");

                await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                Console.WriteLine(json);
            }
        }
}