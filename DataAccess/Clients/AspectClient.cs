using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ogybot.DataAccess.Entities;
using ogybot.DataAccess.Security;
using ogybot.Util;

namespace ogybot.DataAccess.Clients;

/// <summary>
/// Class responsible for handling requests and responses from external tome API
/// </summary>
public class AspectClient
{
    // Needs to be cleaned up

    private const string Endpoint = "aspects";

    private readonly HttpClient _client;
    private readonly TokenGenerator _tokenGenerator;

    public AspectClient(HttpClient client, TokenGenerator tokenGenerator)
    {
        _client = client;
        _tokenGenerator = tokenGenerator;
    }

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

        // Get token, validate if it's null and add to headers
        var token = await _tokenGenerator.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            return new Response("", false, ErrorMessages.GetTokenError);
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Make request
        var response = await _client.PostAsync(Endpoint, content);

        // Return true if success status code, else return false and an error.
        return response.IsSuccessStatusCode
            ? new Response("", true)
            : new Response("", false, ErrorMessages.DecrementUserAspectsError);
    }
}