using Discord.Interactions;

namespace ogybot.Commands.Other;

/// <summary>
/// zingus
/// </summary>
public class ZingusCommand : BaseCommand
{
    [SlashCommand("zingus", "zingusposts")]
    public async Task ExecuteCommandAsync()
    {
        await DeferAsync();

        await FollowupAsync("https://tenor.com/view/zingus-cat-kitty-caption-cat-walk-gif-19570879");
    }
}