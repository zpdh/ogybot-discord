using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ogybot.Data.Clients;

public abstract class BaseClient
{
    private readonly HttpClient _httpClient;

    public BaseClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected async Task<HttpResponseMessage> MakeRequestAsync(string endpoint, HttpMethod method, object? content = null, string? token = null)
    {
        var request = new HttpRequestMessage(method, endpoint);

        AddOptionalFieldsToRequest(content, token, request);

        return await SendHttpRequest(request);
    }

    private static void AddOptionalFieldsToRequest(object? content, string? token, HttpRequestMessage request)
    {
        if (content != null)
        {
            AddContentToRequest(content, request);
        }

        if (token != null)
        {
            AddTokenToHeader(token, request);
        }
    }

    private static void AddContentToRequest(object? content, HttpRequestMessage request)
    {
        request.Content = JsonContent.Create(content);
    }

    private static void AddTokenToHeader(string? token, HttpRequestMessage request)
    {
        if (request.Headers.Contains("Authorization"))
        {
            request.Headers.Remove("Authorization");
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<HttpResponseMessage> SendHttpRequest(HttpRequestMessage request)
    {
        return await _httpClient.SendAsync(request);
    }
}