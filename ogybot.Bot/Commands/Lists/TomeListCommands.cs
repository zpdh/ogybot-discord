using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using ogybot.Bot.Commands.Base;
using ogybot.Communication.Constants;
using ogybot.Communication.Exceptions;
using ogybot.Domain.Clients;
using ogybot.Domain.Entities;

namespace ogybot.Bot.Commands.Lists;

public class TomeListCommands : BasePermissionRequiredCommand
{
    private readonly ITomeListClient _tomeListClient;
    private readonly string _validCharacters;

    public TomeListCommands(ITomeListClient tomeListClient, IConfiguration configuration)
    {
        _tomeListClient = tomeListClient;
        _validCharacters = configuration.GetValue<string>("ValidCharacters")!;
    }

    #region List Command

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("tomelist", "Presents the tome list to get a guild tome.")]
    public async Task ExecuteTomeListCommandAsync()
    {
        if (await IsInvalidChannelAsync(GuildChannels.LayoffsChannel))
        {
            return;
        }

        var embed = await CreateEmbedAsync();

        await FollowupAsync(embed: embed);
    }

    private async Task<Embed> CreateEmbedAsync()
    {
        var content = await GetEmbedContentAsync();

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(content.SocketUser.Username, content.SocketUser.GetAvatarUrl() ?? content.SocketUser.GetDefaultAvatarUrl())
            .WithTitle("Tome List")
            .WithDescription(content.Description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter(content.QueueSize);

        return embedBuilder.Build();
    }

    private async Task<EmbedContent> GetEmbedContentAsync()
    {
        var list = await _tomeListClient.GetListAsync();

        var user = Context.User;
        var queueSize = "Players in queue: " + list.Count;
        var description = CreateEmbedDescription(list);

        return new EmbedContent(user, queueSize, description);
    }

    private static string CreateEmbedDescription(IList<TomeListUser> list)
    {
        var description = "";

        var counter = 1;

        foreach (var tomeListUser in list)
        {
            description += $"{counter}. {tomeListUser.Username}\n";

            counter++;
        }

        return description;
    }

    #endregion

    #region Add To List Command

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("tomelist-add", "Adds a user to the tome list.")]
    public async Task ExecuteTomeListAddCommandAsync([Summary("user", "User to insert into the tome list")] string username)
    {
        if (await IsInvalidChannelAsync(GuildChannels.LayoffsChannel))
        {
            return;
        }

        ValidateUsername(username);

        await AddUserToTomeListAsync(username);

        await FollowupAsync($"Successfully added player {username} to the wait list.");
    }

    private async Task AddUserToTomeListAsync(string username)
    {
            var tomeListUser = new TomeListUser(username);

            await _tomeListClient.AddUserAsync(tomeListUser);
    }

    private void ValidateUsername(string username)
    {
        if (username.Any(character => !_validCharacters.Contains(character)))
        {
            throw new InvalidCommandArgumentException();
        }
    }

    #endregion

    #region Remove User From List Command

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("tomelist-remove", "removes a user from the tome list based on their name or index")]
    public async Task ExecuteTomeListRemoveCommandAsync([Summary("user-or-index", "The user's name or index")] string usernameOrIndex)
    {
        if (await IsInvalidContextAsync(GuildChannels.LayoffsChannel))
        {
            return;
        }

        await RemovePlayerFromListAsync(usernameOrIndex);

        await FollowupAsync($"Successfully removed provided player from the wait list.");
    }

    private async Task RemovePlayerFromListAsync(string usernameOrIndex)
    {

        if (short.TryParse(usernameOrIndex, out var index))
        {
            await RemoveByIndexAsync(index);
        }
        else
        {
            await RemoveByNameAsync(usernameOrIndex);
        }
    }

    private async Task RemoveByNameAsync(string username)
    {
        var tomeListUser = new TomeListUser(username);

        await _tomeListClient.RemoveUserAsync(tomeListUser);
    }

    private async Task RemoveByIndexAsync(int index)
    {
        var list = await _tomeListClient.GetListAsync();

        // Gets the user based on the index provided. As the list count starts at 1, the index has to be subtracted by 1.
        var tomeListUser = list[index - 1];

        await _tomeListClient.RemoveUserAsync(tomeListUser);
    }

    #endregion

}