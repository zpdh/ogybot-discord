using Microsoft.Extensions.Configuration;
using ogybot.Communication.Constants;
using ogybot.Communication.Exceptions;
using ogybot.Domain.Entities;

// ReSharper disable SimplifyLinqExpressionUseAll

namespace ogybot.Bot.Commands.Lists.Validators;

public interface IListCommandValidator
{
    void ValidateUsername(string username);
    void ValidateUserRemoval(IList<BaseUser> userList, string username);
    void ValidateUserRemoval(IList<BaseUser> userList, int index);
}

public class ListCommandValidator : IListCommandValidator
{
    private readonly string _validCharacters;

    public ListCommandValidator(string validCharacters)
    {
        _validCharacters = validCharacters;
    }

    public void ValidateUsername(string username)
    {
        if (username.Any(character => !_validCharacters.Contains(character)))
        {
            throw new InvalidCommandArgumentException(ErrorMessages.InvalidCharactersError);
        }

        if (username.Length > 32)
        {
            throw new InvalidCommandArgumentException(ErrorMessages.UsernameTooLongError);
        }
    }

    public void ValidateUserRemoval(IList<BaseUser> userList, string username)
    {
        if (!userList.Any(user => user.Username == username))
        {
            throw new InvalidCommandArgumentException(ErrorMessages.UsernameNotOnListError);
        }
    }

    public void ValidateUserRemoval(IList<BaseUser> userList, int index)
    {
        // The user index starts at 1 whilst the list index starts at 0, not to cause confusion.
        if (index <= 0 || index > userList.Count)
        {
            throw new InvalidCommandArgumentException(ErrorMessages.InvalidIndexError);
        }
    }
}