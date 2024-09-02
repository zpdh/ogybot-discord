using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace test.Commands.Other;

public abstract class WarQuestionCommand : ICommand
{
    public static async Task ExecuteCommandAsync(SocketSlashCommand command)
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
            .WithColor(Color.Teal)
            .WithCurrentTimestamp();

        var channel = command.Channel as ITextChannel;
        var newThread = await channel!.CreateThreadAsync($"{user.GlobalName}'s Build Help");
        await newThread.SendMessageAsync(embed: embedBuilder.Build());
        await Task.Delay(200);
        await newThread.SendMessageAsync(text: $"||<@&1255013857995395094><@{user.Id}>||", allowedMentions: AllowedMentions.All);
        await command.FollowupAsync("Successfully started thread.", ephemeral: true);
    }

    public static async Task GenerateCommandAsync(DiscordSocketClient socketClient, ulong guildId)
    {
        //War question cmd
        try
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName("war-build-help")
                .WithDescription("Put up a build help thread in war questions")
                .AddOption("classes", ApplicationCommandOptionType.String, "Your available classes", isRequired: true)
                .AddOption("mythics", ApplicationCommandOptionType.String, "The mythics you own", isRequired: true)
                .AddOption("budget", ApplicationCommandOptionType.String, "Your budget in LE", isRequired: true);

            await socketClient.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}