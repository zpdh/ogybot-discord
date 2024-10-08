﻿namespace ogybot.Util;

/// <summary>
/// Class used to store common error messages used throughout the entire application
/// </summary>
public class ErrorMessages
{
    public const string InvalidChannelError = "This command cannot be used in this channel";
    public const string NoPermissionError = "You don't have permissions to use this command";

    public const string GetTokenError = "Error getting token";
    public const string AddUserToListError = "Error adding user to list";
    public const string DecrementUserAspectsError = "Error decrementing user aspect count";
    public const string RemoveUserFromListError = "Error removing user from list";
}