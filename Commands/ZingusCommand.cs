using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using test.SlashCommands;

namespace test.Commands;

public class ZingusCommand : ICommand
{
         public static async Task ExecuteCommandAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("https://tenor.com/view/zingus-cat-kitty-caption-cat-walk-gif-19570879");
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