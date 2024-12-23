using Discord;
using Discord.Interactions;
using ogybot.Bot.Handlers;
using ogybot.Domain.Entities;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Groups.Waitlist;

public sealed class WaitlistListCommand : BaseWaitlistCommand
{
    public WaitlistListCommand(
        IBotExceptionHandler exceptionHandler,
        IGuildClient guildClient,
        IWaitListClient waitListClient) : base(exceptionHandler, guildClient, waitListClient)
    {
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("list", "Shows the list to rejoin the guild.")]
    public async Task ExecuteCommandAsync()
    {

        await HandleCommandExecutionAsync(CommandInstructionsAsync);
    }

    private async Task CommandInstructionsAsync()
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        var embed = await CreateEmbedAsync();

        await FollowupAsync(embed: embed);
    }

    private async Task<Embed> CreateEmbedAsync()
    {
        // Create class to store this info later
        var content = await GetEmbedContentAsync();

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(content.User.Username, content.User.GetAvatarUrl() ?? content.User.GetDefaultAvatarUrl())
            .WithTitle("Wait list")
            .WithDescription(content.Description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter(content.QueueSize);

        return embedBuilder.Build();
    }

    private async Task<EmbedContent> GetEmbedContentAsync()
    {
        var list = await WaitListClient.GetListAsync(WynnGuildId);

        var user = Context.User;
        var queueSize = "Players in queue: " + list.Count;
        var description = CreateEmbedDescription(list);

        return EmbedContent.Create(user, queueSize, description);
    }

    private static string CreateEmbedDescription(IList<WaitListUser> list)
    {
        var description = "";

        var counter = 1;

        foreach (var userWaitlist in list)
        {
            description += $"{counter}. {userWaitlist.Username}\n";

            counter++;
        }

        return description;
    }
}