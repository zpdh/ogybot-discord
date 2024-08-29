using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using test.Api.Entities;

namespace test.Api.Repositories;

public class AspectClient
{
    // Needs to be cleaned up

    private const string Endpoint = "aspects";

    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("https://ico-server.onrender.com/")
    };

    public async Task<List<UserAspectlist>?> GetAspectsOwedListAsync()
    {
        var userList = await _client.GetFromJsonAsync(
            Endpoint,
            typeof(List<UserAspectlist>));

        return userList as List<UserAspectlist>;
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
            return new Response("", false);
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsync(Endpoint, content);

        if (response.IsSuccessStatusCode)
        {
            return new Response("", true);
        }

        Console.WriteLine("Error retrieving aspect player list");
        return new Response("", false);
    }

    private async Task<string?> GetTokenAsync()
    {
        const string validationKey = "P?DmuA*y7nsqHnt}.&2;pQ";

        // Convert to object since the endpoint
        // demands a key:value pair
        var json = JsonConvert.SerializeObject(new
        {
            validationKey = validationKey
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
            var apiResponse = await response.Content.ReadFromJsonAsync<AspectApiResponse>();
            return apiResponse!.Token;
        }

        Console.WriteLine("Error getting token");
        Console.WriteLine(await response.Content.ReadAsStringAsync());

        return null;
    }
}