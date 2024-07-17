using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using test.Api;
using test.SlashCommands;

namespace test.Commands.Waitlist;

public class WaitlistCommand : ICommand
{
    private static WaitlistController _controller = new();
    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        
        var user = command.User;
        var list = await _controller.GetWaitlistAsync();
        var description = "";
        var queueSize = "Players in queue: " + list.Count();

        var i = 1;
        foreach (var u in list)
        {
            description += i + ". " + u.UserName.ToString() + "\n";
            i++;
        }
        
        var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle($"Waitlist")
            .WithDescription(description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter(queueSize);

        await command.RespondAsync(embed: embedBuilder.Build());
        
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("waitlist")
                .WithDescription("Displays waitlist");
            await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}