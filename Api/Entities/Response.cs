namespace test.Api.Entities;

public class Response
{
    public string Username { get; }
    public bool Status { get; }
    
    public Response(string username, bool status)
    {
        Username = username;
        Status = status;
    }
}