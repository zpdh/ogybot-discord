using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace ogybot.Commands.Other;

public abstract class PingChiefsCommand : ICommand
{
    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        await command.FollowupAsync("<@&1097935496442810419> Wake up", allowedMentions: AllowedMentions.All);
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("chiefs")
                .WithDescription("Pings active chiefs (use if you need somebody to set headquarters)");

            await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}