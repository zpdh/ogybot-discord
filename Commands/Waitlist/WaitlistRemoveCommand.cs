using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using ogybot.DataAccess.Controllers;
using ogybot.DataAccess.Entities;

namespace ogybot.Commands.Waitlist;

public abstract class WaitlistRemoveCommand : ICommand
{
    private static readonly WaitlistController Controller = new();

    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
    {
        var inputAsString = command.Data.Options.FirstOrDefault()!.Value.ToString();

        // Checks if input can be converted to an integer. If so, removes user by index instead of name.
        if (int.TryParse(inputAsString, out var index))
        {
            await RemoveByIndex(command, index);
            return;
        }

        await RemoveByName(command, inputAsString!);
    }

    private static async Task RemoveByName(SocketSlashCommand command, string username)
    {
        var result = await Controller.RemovePlayerAsync(new UserWaitlist { Username = username });

        var msg = result.Status
            ? $"Successfully removed player '{result.Username}' from the wait list"
            : $"User '{result.Username}' is not on the wait list";

        await command.FollowupAsync(msg);
    }

    private static async Task RemoveByIndex(SocketSlashCommand command, int index)
    {
        var list = await Controller.GetWaitlistAsync();

        var username = list[index - 1].Username;
        var result = await Controller.RemovePlayerAsync(new UserWaitlist { Username = username });

        var msg = result.Status
            ? $"Successfully removed player '{result.Username}' from the wait list"
            : $"User '{result.Username}' is not on the wait list";

        await command.FollowupAsync(msg);
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("waitlist-remove")
                .WithDescription("Removes user from wait list")
                .AddOption(
                    "username-or-index",
                    ApplicationCommandOptionType.String,
                    "User you're removing or their index on the list", true);
            await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}