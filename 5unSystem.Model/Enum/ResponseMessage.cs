using System;

namespace _5unSystem.Model.Enum;

public class ResponseMessage
{
    public const string SUCCESS = "Success";
}

public class ResponseLoginMessage
{
    public const string USER_NOT_FOUND = "User not found";
    public const string INVALID_CREDENTIALS = "Username or password Invalid";
    public const string USERNAME_OR_PASSWORD_EMPTY = "Username or password cannot be empty";
}