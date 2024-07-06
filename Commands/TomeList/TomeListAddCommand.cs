using Discord.Net;
using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;
using test.Services.Api;
using test.SlashCommands;

namespace test.Commands
{
    internal class TomeListAddCommand : ICommand
    {
        public static async Task ExecuteCommandAsync(SocketSlashCommand command)
        {
            /*
            var user = command.User;
            var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle($"Waitlist list")
            .WithDescription("placebo")
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter("placebo");

            await command.RespondAsync(embed: embedBuilder.Build());
            */
            var username = command.Data.Options.FirstOrDefault().Value;
            if (username == null) await command.RespondAsync("Failed to add user", ephemeral: true);
            else
            {
                await ApiRequestService.CreateUserAsync(new User() {Name = username.ToString()});
                await command.RespondAsync("Successfully added user", ephemeral: true);
            }
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
