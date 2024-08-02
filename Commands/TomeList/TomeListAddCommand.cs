using Discord.Net;
using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;
using test.Api;
using test.Api.Controllers;
using test.Services;
using test.SlashCommands;

namespace test.Commands
{
    internal class TomeListAddCommand : ICommand
    {
        private static TomelistController _controller = new();
        
        public static async Task ExecuteCommandAsync(SocketSlashCommand command)
        {
            var username = command.Data.Options.FirstOrDefault().Value;

            var result = await _controller.AddPlayerAsync(new UserBase { Username = username.ToString() });
            
            await command.RespondAsync(result);
        }

        public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
        {
            try
            {
                var guildCommand = new SlashCommandBuilder()
                    .WithName("tomelist-add")
                    .WithDescription("Adds user to tome list")
                    .AddOption("username", ApplicationCommandOptionType.String, "User you're adding", true);
                await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                Console.WriteLine(json);
            }
        }
    }
}
