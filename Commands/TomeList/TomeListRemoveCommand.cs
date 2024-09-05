using Discord;
using Discord.Interactions;
using ogybot.DataAccess.Controllers;
using ogybot.DataAccess.Entities;
using ogybot.Util;

namespace ogybot.Commands.TomeList;

public class TomeListRemoveCommand : BaseRemoveCommand
{
    private readonly TomelistController _controller;

    public TomeListRemoveCommand(TomelistController controller)
    {
        _controller = controller;
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("tomelist-remove", "removes user from tomelist by name or index")]
    public async Task ExecuteCommandAsync([Summary("user-or-index", "the user or their index")] string input)
    {
        if (await ValidateChannelAndRolesAsync(GuildChannels.TomeChannel)) return;

        if (input.Contains(' ') && !input.Contains(','))
        {
            await FollowupAsync("You cannot submit usernames with whitespaces");
            return;
        }

        var inputList = input.Split(',')
            .Select(user => user.Trim())
            .Distinct()
            .ToList();

        var responseList = new List<Response>();
        var indexList = new List<int>();

        foreach (var singleInput in inputList)
        {
            // Check if the input can be converted to an integer. if so, remove user in that position in the list.
            if (int.TryParse(singleInput, out var index))
            {
                indexList.Add(index);
            }
            else
            {
                // In this case, the input is the user's name
                await RemoveByName(singleInput, responseList);
            }
        }

        if (indexList.Any())
        {
            await RemoveByIndex(indexList, responseList);
        }

        await GenerateFollowupMessage(responseList);
    }

    private async Task GenerateFollowupMessage(List<Response> responseList) {
        var errorList = responseList
            .Select(response => response.Error)
            .Where(error => error is not null)
            .Distinct()
            .ToList();

        if (errorList.Any())
        {
            var formattedErrorList = errorList.Aggregate("", (current, error) => current + $"'{error}'" + ", ");

            await FollowupAsync($"One or multiple errors occurred: {formattedErrorList[..^2]}");
            return;
        }

        var users = responseList
            .Select(user => user.Username)
            .Aggregate("", (current, user) => current + ($"'{user}'" + ", "));

        // [..^n] removes the last n characters of an array
        var msg = $"Successfully removed players {users[..^2]} from the tome list.";

        await FollowupAsync(msg);
    }

    private async Task RemoveByName(string username, List<Response> responseList)
    {
        var result = await _controller.RemovePlayerAsync(new UserTomelist { Username = username });

        responseList.Add(result);
    }

    private async Task RemoveByIndex(List<int> indexList, List<Response> responseList)
    {
        indexList = indexList.OrderDescending().ToList();

        var list = await _controller.GetTomelistAsync();

        var listOfUsernames = indexList.Select(index => list[index - 1].Username!);

        foreach (var username in listOfUsernames)
        {
            var result = await _controller.RemovePlayerAsync(new UserTomelist { Username = username });
            responseList.Add(result);
        }
    }
}