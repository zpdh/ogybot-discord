namespace ogybot.DataAccess.Entities;

/// <summary>
/// Represents the response of a token API request.
/// </summary>
public record TokenApiResponse
{
    public bool Status { get; init; }
    public string? Token { get; init; }
    public string? Error { get; init; }
}