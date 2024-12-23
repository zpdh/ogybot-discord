using Discord;
using Discord.Interactions;
using ogybot.Bot.Commands.Base;
using ogybot.Bot.Handlers;
using ogybot.Domain.Entities;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Misc;

public class OnlineCommand : BaseCommand
{
    private readonly IOnlineClient _client;

    private ulong ValidChannelId { get; set; }

    public OnlineCommand(
        IOnlineClient client,
        IBotExceptionHandler exceptionHandler,
        IGuildClient guildClient) : base(exceptionHandler, guildClient)
    {
        _client = client;
    }

    protected override void ConfigureCommandSettings()
    {
        ValidChannelId = ServerConfiguration.ListeningChannel;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("online", "Lists online players with the mod.")]
    public async Task ExecuteCommandAsync()
    {
        if (await IsInvalidChannelAsync(ValidChannelId))
        {
            return;
        }

        await HandleCommandExecutionAsync(CommandInstructionsAsync);
    }

    private async Task CommandInstructionsAsync()
    {
        var embed = await CreateEmbedAsync();

        await FollowupAsync(embed: embed);
    }

    private async Task<Embed> CreateEmbedAsync()
    {
        // Create class to store this info later
        var embedContent = await GetEmbedContentAsync();

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(embedContent.User.Username, embedContent.User.GetAvatarUrl() ?? embedContent.User.GetDefaultAvatarUrl())
            .WithTitle("Online List")
            .WithDescription(embedContent.Description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp();

        return embedBuilder.Build();
    }

    private async Task<EmbedContent> GetEmbedContentAsync()
    {
        var user = Context.User;

        var list = await _client.GetListAsync();


        var description = CreateEmbedDescription(list);

        return EmbedContent.Create(user, description: description);
    }

    private static string CreateEmbedDescription(IList<OnlineUser> list)
    {
        var description = "";

        var counter = 1;

        foreach (var onlineUser in list)
        {
            description += $"{counter}. {onlineUser.Username}\n";

            counter++;
        }

        return description;
    }
}