﻿using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace ogybot.Commands.Other;

public abstract class ListCommand : ICommand
{
    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        var user = command.User;

        /* TODO: Transform these walls of text into a StringBuilder result
         * Seriously. It's awful
         */

        var textContent = "**/raid [Required: Raid Type] [Optional: Guild Name]**\n" +
                          "Pings either heavy/light raid and you may choose whether or not to inform the guild currently attacking ICo.\n\n" +
                          "**/war-build-help [Required: Classes] [Required: Mythics Owned] [Required: Budget]**\n" +
                          "Starts a war question thread regarding builds with all of the info required for an answer from the war trainers.\n\n" +
                          "**/chiefs**\n" +
                          "Pings active chiefs (use if you need somebody to set you a headquarter and/or tributes)\n\n" +
                          "**/tomelist**\n" +
                          "Displays tome list.\n" +
                          "/tomelist-add [Required: username] adds player to queue\n" +
                          "/tomelist-remove [Required: username] removes player from queue.";

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle($"List of commands")
            .WithDescription(textContent)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter($"Bot made by oxzy");

        await command.FollowupAsync(embed: embedBuilder.Build());
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