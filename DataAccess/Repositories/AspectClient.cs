using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using test.DataAccess.Entities;
using test.Util;

namespace test.DataAccess.Repositories;

public class AspectClient
{
    // Needs to be cleaned up

    private const string Endpoint = "aspects";

    private readonly HttpClient _client = new()
    {
        BaseAddress = CommonConstants.ApiUri
    };

    public async Task<List<UserAspectlist>?> GetAspectsOwedListAsync()
    {
        var userJson = await _client.GetAsync(
            Endpoint);

        var userList = JsonConvert.DeserializeObject<List<UserAspectlist>>(
            await userJson.Content.ReadAsStringAsync());

        return userList;
    }

    public async Task<Response> DecrementAspectFromPlayerAsync(IEnumerable<string> players)
    {
        players = players.ToList();

        // Convert to object since the endpoint
        // demands a key:value pair
        var json = JsonConvert.SerializeObject(new
        {
            users = players
        });

        // Convert to string content in order
        // to post
        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        );

        var token = await GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            return new Response("", false, ErrorMessages.GetTokenError);
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsync(Endpoint, content);

        if (response.IsSuccessStatusCode)
        {
            return new Response("", true);
        }

        Console.WriteLine("Error decrementing aspect");
        return new Response("", false, ErrorMessages.DecrementUserAspectsError);
    }

    private async Task<string?> GetTokenAsync()
    {
        // Convert to object since the endpoint
        // demands a key:value pair
        var json = JsonConvert.SerializeObject(new
        {
            validationKey = CommonConstants.ValidationKey
        });

        // Convert to string content in order
        // to post
        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        );

        var response = await _client.PostAsync("auth/gettoken", content);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var apiResponse = await response.Content.ReadFromJsonAsync<TokenApiResponse>();
            return apiResponse!.Token;
        }

        Console.WriteLine("Error getting token");
        Console.WriteLine(await response.Content.ReadAsStringAsync());

        return null;
    }
}