using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.SlashCommands
{
    public class ListCommand : ICommand
    {
        public static async Task ExecuteCommandAsync(SocketSlashCommand command)
        {
            var user = command.User;

            var textContent = "**/raid [Required: Raid Type] [Optional: Guild Name]**\n" +
                "Pings either heavy/light raid and you may choose whether or not to inform the guild currently attacking ICo.\n\n" +
                "**/war-build-help [Required: Classes] [Required: Mythics Owned] [Required: Budget]**\n" +
                "Starts a war question thread regarding builds with all of the info required for an answer from the war trainers.\n\n" +
                "**/chiefs**\n" +
                "Pings active chiefs (use if you need somebody to set you a headquarter and/or tributes)";

            var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle($"List of commands")
            .WithDescription(textContent)
            .WithColor(Color.Purple)
            .WithCurrentTimestamp()
            .WithFooter($"Bot made by oxzy");

            await command.RespondAsync(embed: embedBuilder.Build());
        }

        public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
        {
            try
            {
                var guildCommand = new SlashCommandBuilder()
                .WithName("ogy-cmdlist")
                .WithDescription("Provides a list of commands for ogybot");

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
