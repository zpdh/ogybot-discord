using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using test.DataAccess.Entities;
using test.Util;

namespace test.DataAccess.Repositories;

/// <summary>
/// Class responsible for handling requests and responses from external tome API
/// </summary>
public class TomeClient
{
    private const string Endpoint = "tomes";

    private readonly HttpClient _client = new()
    {
        BaseAddress = CommonConstants.ApiUri
    };

    /// <summary>
    /// Gets users queued on the tome list
    /// </summary>
    /// <returns>
    /// List of users in tome queue
    /// </returns>
    public async Task<List<UserTomelist>> GetListAsync()
    {
        var listJson = await _client.GetAsync(Endpoint);

        var listOfUsers = JsonConvert
            .DeserializeObject<List<UserTomelist>>(
                await listJson.Content.ReadAsStringAsync());

        return listOfUsers!;
    }

    /// <summary>
    /// Adds user to tome queue
    /// </summary>
    /// <param name="user">User to be added on the list</param>
    /// <returns>Response of type <see cref="Response"/></returns>
    public async Task<Response> PostUserAsync(UserTomelist user)
    {
        // Convert object to json and send
        // username for verification
        var json = JsonConvert.SerializeObject(new
        {
            username = user.Username
        });

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        // Get token and add to headers
        var token = await GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            return new Response("", false, ErrorMessages.GetTokenError);
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Send request to API
        var response = await _client.PostAsync(Endpoint, content);

        if (response.IsSuccessStatusCode)
        {
            return new Response(user.Username!, true);
        }

        Console.WriteLine("Error posting user to tome list");
        return new Response(user.Username!, false, ErrorMessages.AddUserToListError);
    }

    /// <summary>
    /// Removes user from tome queue
    /// </summary>
    /// <param name="user">User to be removed</param>
    /// <returns>Response of type <see cref="Response"/></returns>
    public async Task<Response> RemoveUserAsync(UserTomelist user)
    {
        var json = JsonConvert.SerializeObject(new
        {
            username = user.Username
        });

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        // Get token and add to headers
        var token = await GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            return new Response("", false, ErrorMessages.GetTokenError);
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.DeleteAsync($"{Endpoint}/{user.Username}");

        if (response.IsSuccessStatusCode)
        {
            return new Response(user.Username!, true);
        }

        Console.WriteLine("Error removing user from tome list");
        return new Response("", false, ErrorMessages.RemoveUserFromListError);
    }

    /// <summary>
    /// Requests and gets a validation token from API
    /// </summary>
    /// <returns>
    /// Token or null if request is invalid
    /// </returns>
    private async Task<string?> GetTokenAsync()
    {
        // Serialize validation key to JSON
        var json = JsonConvert.SerializeObject(new
        {
            validationKey = CommonConstants.ValidationKey
        });

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        // Get token response from API
        var response = await _client.PostAsync("auth/gettoken", content);

        if (response.StatusCode.Equals(HttpStatusCode.OK))
        {
            var apiResponse = await response.Content.ReadFromJsonAsync<TokenApiResponse>();
            return apiResponse!.Token;
        }

        Console.WriteLine("Error getting token");
        return null;
    }
}