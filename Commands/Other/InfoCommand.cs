using System.Text;
using Discord;
using Discord.Interactions;

namespace ogybot.Commands.Other;

/// <summary>
/// Displays info about the bot
/// </summary>
public class InfoCommand : BaseCommand
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("info", "displays info about the bot")]
    public async Task ExecuteCommandAsync()
    {
        const string title = "title";
        const string footer = "Made by oxzy";

        var user = Context.User;

        var description = new StringBuilder()
            .AppendLine("**Github Repos**")
            .AppendLine("Bot: https://github.com/zpdh/ogybot-discord")
            .AppendLine("API: https://github.com/ezlixp/ico_server")
            .ToString();

        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithFooter(footer)
            .WithDescription(description)
            .WithAuthor(user)
            .WithTimestamp(DateTimeOffset.Now)
            .WithColor(Color.Teal)
            .Build();


        await FollowupAsync(embed: embed);
    }
}