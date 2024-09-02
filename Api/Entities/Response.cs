namespace test.Api.Entities;

/// <summary>
/// Class responsible for handling API responses
/// </summary>
public class Response
{
    public string Username { get; }
    public bool Status { get; }
    public string? Error { get; }

    public Response(string username, bool status, string? error = null)
    {
        Username = username;
        Status = status;
        Error = error;
    }
}