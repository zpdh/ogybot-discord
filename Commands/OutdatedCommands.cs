using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace test.Commands;

public abstract class OutdatedCommands : ICommand
{
    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        await command.FollowupAsync("<@1088133910807969855> lol this person used an outdated command (use /war-build-help)");
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("build-help")
                .WithDescription("Outdated command, use /war-build-help");

            await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}