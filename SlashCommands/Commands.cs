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

namespace test.Commands
{
    public class Commands
    {
        private static DiscordSocketClient _socketClient { get; set; }

        public Commands(DiscordSocketClient socketClient)
        {
            _socketClient=socketClient;
        }

        public async Task Client_Ready()
        {
            ulong guildId = 810258030201143328;
            var guild = _socketClient.GetGuild(guildId);

            //Raid cmd
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

            //War question cmd
            var guildCommand2 = new SlashCommandBuilder()
            .WithName("war-build-help")
            .WithDescription("Put up a build help thread in war questions")
            .AddOption("classes", ApplicationCommandOptionType.String, "Your available classes", isRequired: true)
            .AddOption("mythics", ApplicationCommandOptionType.String, "The mythics you own", isRequired: true)
            .AddOption("budget", ApplicationCommandOptionType.String, "Your budget in LE", isRequired: true);

            //Command list cmd
            var guildCommand3 = new SlashCommandBuilder()
            .WithName("ogy-cmdlist")
            .WithDescription("Provides a list of commands for ogybot");

            try
            {
                await _socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
                await _socketClient.Rest.CreateGuildCommand(guildCommand2.Build(), guildId);
                await _socketClient.Rest.CreateGuildCommand(guildCommand3.Build(), guildId);
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                Console.WriteLine(json);
            }
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "raid":
                    if (command.ChannelId != 863553410813001759) await InvalidChannelCommand(command);
                    else await PingRaidCommand(command);
                    break;

                case "war-build-help":
                    if (command.ChannelId != 1255011451056423013) await InvalidChannelCommand(command);
                    else await WarQuestionCommand(command);
                    break;

                case "ogy-cmdlist":
                    await HelpCommand(command);
                    break;
            }
        }

        // #cmdmethod RAIDPING
        public async Task PingRaidCommand(SocketSlashCommand command)
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

        // #cmdmethod WARQUESTION
        public async Task WarQuestionCommand(SocketSlashCommand command)
        {
            var user = command.User;
            var info = command.Data.Options.ToList();

            var textContent = $"**Classes:** {info[0].Value}\n" +
                $"**Mythics:** {info[1].Value}\n" +
                $"**Budget:** {info[2].Value}\n";

            var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle($"{user.GlobalName}'s Build Help")
            .WithDescription(textContent)
            .WithColor(Color.Purple)
            .WithCurrentTimestamp();

            var channel = command.Channel as ITextChannel;
            var newThread = await channel.CreateThreadAsync($"{user.GlobalName}'s Build Help");
            await newThread.SendMessageAsync(embed: embedBuilder.Build());
            await Task.Delay(200);
            await newThread.SendMessageAsync(text: $"||<@&1255013857995395094><@{user.Id}>||", allowedMentions: AllowedMentions.All);
            await command.RespondAsync("Successfully started thread.", ephemeral: true);
        }

        // #cmdthod HELP
        public async Task HelpCommand(SocketSlashCommand command)
        {
            var user = command.User;

            var textContent = "**/raid [Required: Raid Type] [Optional: Guild Name]**\n" +
                "Pings either heavy/light raid and you may choose whether or not to inform the guild currently attacking ICo.\n\n" +
                "**/war-build-help [required: Classes] [Required: Mythics Owned] [Required: Budget]**\n" +
                "Starts a war question thread regarding builds with all of the info required for an answer from the war trainers. ";

            var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle($"List of commands")
            .WithDescription(textContent)
            .WithColor(Color.Purple)
            .WithCurrentTimestamp()
            .WithFooter($"Bot made by oxzy");

            await command.RespondAsync(embed: embedBuilder.Build());
        }

        public async Task InvalidChannelCommand(SocketSlashCommand command)
        {
            var userId = command.User.Id;
            await command.RespondAsync($"This is not a valid channel for this command , <@{userId}>!", ephemeral: true);
        }
    }
}
