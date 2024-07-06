using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using test.Services.Api;
using test.SlashCommands;

namespace test.Commands.TomeList;

public class TomeListRemoveCommand : ICommand
{
    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        var username = command.Data.Options.FirstOrDefault().Value;
        if (username == null) await command.RespondAsync("Failed to remove user", ephemeral: true);
        else
        {
            await ApiRequestService.DeleteUserAsync(new User() { Name = username.ToString() });
            await command.RespondAsync("Successfully removed user", ephemeral: true);
        }
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("tomelist-remove")
                .WithDescription("Removes user from tome list")
                .AddOption("username", ApplicationCommandOptionType.String, "User you're removing", true);
            await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}