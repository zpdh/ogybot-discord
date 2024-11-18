﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using ogybot.Bot.Commands.Base;
using ogybot.Communication.Constants;
using ogybot.Domain.Clients;
using ogybot.Domain.Entities;

namespace ogybot.Bot.Commands.Lists;

public class AspectListCommands : BasePermissionRequiredCommand
{

    private readonly IAspectListClient _aspectListClient;

    public AspectListCommands(IAspectListClient aspectListClient)
    {
        _aspectListClient = aspectListClient;
    }

    #region List Command

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("aspectlist", "Presents the aspect list to get a guild aspect.")]
    public async Task ExecuteAspectListCommandAsync()
    {
        if (await IsInvalidChannelAsync(GuildChannels.RaidsChannel))
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
            .WithTitle("Aspect List")
            .WithDescription(content.Description)
            .WithColor(Color.Teal)
            .WithCurrentTimestamp()
            .WithFooter(content.QueueSize);

        return embedBuilder.Build();
    }

    private async Task<EmbedContent> GetEmbedContentAsync()
    {
        var list = await _aspectListClient.GetListAsync();

        var user = Context.User;
        var queueSize = "Players in queue: " + list.Count;
        var description = CreateEmbedDescription(list);

        return new EmbedContent(user, queueSize, description);
    }

    private static string CreateEmbedDescription(IList<AspectListUser> list)
    {
        var description = "";

        var counter = 1;

        foreach (var aspectListUser in list)
        {
            description += $"{counter}. {aspectListUser.Username}\n";

            counter++;
        }

        return description;
    }

    #endregion

    #region Decrement Aspect Command

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("aspectlist-decrement", "Decrements an aspect from the provided user.")]
    public async Task ExecuteAspectListDecrementCommandAsync([Summary("user-or-index", "The user's name or index")] string usernameOrIndex)
    {
        if (await IsInvalidContextAsync(GuildChannels.RaidsChannel))
        {
            return;
        }

        await DecrementAspectFromPlayerAsync(usernameOrIndex);

        await FollowupAsync($"Successfully decremented 1 aspect from the provided player.");
    }

    private async Task DecrementAspectFromPlayerAsync(string usernameOrIndex)
    {
        if (short.TryParse(usernameOrIndex, out var index))
        {
            await DecrementByIndexAsync(index);
        }
        else
        {
            await DecrementByNameAsync(usernameOrIndex);
        }
    }

    private async Task DecrementByNameAsync(string username)
    {
        var aspectListUser = new AspectListUser(username);

        await _aspectListClient.DecrementAspectAsync(aspectListUser);
    }

    private async Task DecrementByIndexAsync(int index)
    {
        var list = await _aspectListClient.GetListAsync();

        // Gets the user based on the index provided. As the list count starts at 1, the index has to be subtracted by 1.
        var aspectListUser = list[index - 1];

        await _aspectListClient.DecrementAspectAsync(aspectListUser);
    }

    #endregion

}