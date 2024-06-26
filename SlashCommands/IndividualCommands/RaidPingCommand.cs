using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.SlashCommands.IndividualCommands
{
    public class RaidPingCommand : ICommand
    {
        public static async Task ExecuteCommandAsync(SocketSlashCommand command)
        {
            var info = command.Data.Options.ToList();
            var raidType = info[0].Value;
            try
            {
                var guildAttacking = info[1].Value;

                //Heavy raid
                if (raidType.ToString() == "1")
                    await command.RespondAsync($"<@&1044407292340211793>\nGuild: {guildAttacking}", allowedMentions: AllowedMentions.All);

                //Light raid
                else
                    await command.RespondAsync($"<@&1044407413345886238>\nGuild: {guildAttacking}", allowedMentions: AllowedMentions.All);
            }
            catch
            {
                if (raidType.ToString() == "1")
                    await command.RespondAsync("<@&1044407292340211793>\nGuild not specified.", allowedMentions: AllowedMentions.All);
                else
                    await command.RespondAsync("<@&1044407413345886238>\nGuild not specified.", allowedMentions: AllowedMentions.All);
            }
        }

        public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
        {
            try
            {
                var guildCommand = new SlashCommandBuilder()
                        .WithName("raid")
                        .WithDescription("Pings heavy/light raid role")
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("raidtype")
                            .WithDescription("Heavy/Light Raid")
                            .WithRequired(true)
                            .AddChoice("Heavy Raid", 1)
                            .AddChoice("Light Raid", 2)
                            .WithType(ApplicationCommandOptionType.Integer))
                        .AddOption("guild", ApplicationCommandOptionType.String, "The guild attacking us", isRequired: false);

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
