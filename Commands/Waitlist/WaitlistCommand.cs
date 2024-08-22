using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using test.Api.Controllers;

namespace test.Commands.Waitlist;

public abstract class WaitlistCommand : ICommand
{
    private static readonly WaitlistController Controller = new();

    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        var user = command.User;

        var list = await Controller.GetWaitlistAsync();

        var description = "";

        var queueSize = "Players in queue: " + list.Count;

        for (var i = 1; i <= list.Count; i++)
        {
            description += i + ". " + list[i - 1].Username + "\n";
        }

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle("Wait list")
            .WithDescription(description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter(queueSize);

        await command.FollowupAsync(embed: embedBuilder.Build());
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("waitlist")
                .WithDescription("Displays wait list");
            await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}