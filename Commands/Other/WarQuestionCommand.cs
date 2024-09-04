using Discord;
using Discord.Interactions;
using ogybot.Util;

namespace ogybot.Commands.Other;

/// <summary>
/// Creates a thread for war question purposes.
/// </summary>
public class WarQuestionCommand : BaseCommand
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("war-build-help", "creates a thread for build help")]
    public async Task ExecuteCommandAsync(
        [Summary("classes", "the classes you have available")]
        string classes,
        [Summary("mythics", "the mythics you have available")]
        string mythics,
        [Summary("budget", "your total budget in LE")]
        string budget)
    {
        await DeferAsync();

        if (await ValidateChannelAsync(GuildChannels.WarQuestionsChannel)) return;

        var user = Context.User;

        var textContent = $"**Classes:** {classes}\n" +
                          $"**Mythics:** {mythics}\n" +
                          $"**Budget:** {budget}\n";

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle($"{user.GlobalName}'s Build Help")
            .WithDescription(textContent)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp();

        var channel = Context.Channel as ITextChannel;
        var newThread = await channel!.CreateThreadAsync($"{user.GlobalName}'s Build Help");
        await newThread.SendMessageAsync(embed: embedBuilder.Build());
        await Task.Delay(200);
        await newThread.SendMessageAsync(text: $"||<@&1255013857995395094><@{user.Id}>||",
            allowedMentions: AllowedMentions.All);

        await FollowupAsync("Successfully started thread.", ephemeral: true);
    }
}