namespace test.DataAccess.Entities;

public record TokenApiResponse
{
    public bool Status { get; init; }
    public string? Token { get; init; }
    public string? Error { get; init; }
}