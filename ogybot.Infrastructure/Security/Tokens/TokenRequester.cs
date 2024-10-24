using System.Net.Http.Json;
using ogybot.Communication.Requests;
using ogybot.Domain.Security;

namespace ogybot.Data.Security.Tokens;

public class TokenRequester : ITokenRequester
{
    private readonly HttpClient _httpClient;
    private readonly string _validationKey;

    private const string Endpoint = "auth/gettoken";

    public TokenRequester(HttpClient httpClient, string validationKey)
    {
        _httpClient = httpClient;
        _validationKey = validationKey;
    }
}