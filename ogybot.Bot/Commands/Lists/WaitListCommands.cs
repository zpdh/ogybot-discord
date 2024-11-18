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

public class WaitListCommands : BasePermissionRequiredCommand
{
    private readonly IWaitListClient _waitListClient;
    private readonly string _validCharacters;

    public WaitListCommands(IWaitListClient waitListClient, IConfiguration configuration)
    {
        _waitListClient = waitListClient;
        _validCharacters = configuration.GetValue<string>("ValidCharacters")!;
    }

    #region List Command

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("waitlist", "Presents the wait list to rejoin the guild.")]
    public async Task ExecuteWaitlistCommandAsync()
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
        // Create class to store this info later
        var content = await GetEmbedContentAsync();

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(content.SocketUser.Username, content.SocketUser.GetAvatarUrl() ?? content.SocketUser.GetDefaultAvatarUrl())
            .WithTitle("Wait list")
            .WithDescription(content.Description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter(content.QueueSize);

        return embedBuilder.Build();
    }

    private async Task<EmbedContent> GetEmbedContentAsync()
    {
        var list = await _waitListClient.GetListAsync();

        var user = Context.User;
        var queueSize = "Players in queue: " + list.Count;
        var description = CreateEmbedDescription(list);

        return new EmbedContent(user, queueSize, description);
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

    #endregion

    #region Add To List Command

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("waitlist-add", "Adds a user to the wait list.")]
    public async Task ExecuteWaitlistAddCommandAsync([Summary("user", "User to insert into the wait list")] string username)
    {
        if (await IsInvalidChannelAsync(GuildChannels.LayoffsChannel))
        {
            return;
        }

        ValidateUsername(username);

        await AddUserToWaitlistAsync(username);

        await FollowupAsync($"Successfully added player {username} to the wait list.");
    }

    private async Task AddUserToWaitlistAsync(string username)
    {
        var waitListUser = new WaitListUser(username);

        await _waitListClient.AddUserAsync(waitListUser);
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
    [SlashCommand("waitlist-remove", "removes a user from the wait list based on their name or index")]
    public async Task ExecuteWaitlistRemoveCommandAsync([Summary("user-or-index", "The user's name or index")] string usernameOrIndex)
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
        var waitListUser = new WaitListUser(username);

        await _waitListClient.RemoveUserAsync(waitListUser);
    }

    private async Task RemoveByIndexAsync(int index)
    {
        var list = await _waitListClient.GetListAsync();

        // Gets the user based on the index provided. As the list count starts at 1, the index has to be subtracted by 1.
        var waitListUser = list[index - 1];

        await _waitListClient.RemoveUserAsync(waitListUser);
    }

    #endregion

}