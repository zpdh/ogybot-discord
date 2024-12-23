using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ogybot.Bot.Commands.Base;
using ogybot.Bot.Handlers;
using ogybot.Communication.Constants;
using ogybot.Domain.Entities.UserTypes;
using ogybot.Domain.Infrastructure.Clients;

namespace ogybot.Bot.Commands.Socket;

public class ChatSocketCommands : BaseCommand
{
    private readonly IOnlineClient _client;

    private ulong ValidChannelId { get; set; }

    public ChatSocketCommands(
        IOnlineClient client,
        IBotExceptionHandler exceptionHandler,
        IGuildClient guildClient) : base(exceptionHandler, guildClient)
    {
        _client = client;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("online", "Lists online players with the mod.")]
    public async Task ExecuteOnlineCommandInstructionsAsync()
    {
        var serverConfig = await GetServerConfigurationAsync();
        ValidChannelId = serverConfig.RaidsChannel;

        if (await IsInvalidChannelAsync(GuildChannels.WebsocketLogChannel))
        {
            return;
        }

        await TryExecutingCommandInstructionsAsync(OnlineCommandInstructionsAsync);
    }

    public async Task OnlineCommandInstructionsAsync()
    {
        var embed = await CreateEmbedAsync();

        await FollowupAsync(embed: embed);
    }

    private async Task<Embed> CreateEmbedAsync()
    {
        // Create class to store this info later
        var (user, queueSize, description) = await GetEmbedContentAsync();

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithTitle("Online List")
            .WithDescription(description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp();

        return embedBuilder.Build();
    }

    private async Task<(SocketUser user, string queueSize, string description)> GetEmbedContentAsync()
    {
        var user = Context.User;

        var list = await _client.GetListAsync();

        var queueSize = "Players in queue: " + list.Count;

        var description = CreateEmbedDescription(list);

        return (user, queueSize, description);
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