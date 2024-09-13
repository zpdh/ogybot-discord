﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ogybot.DataAccess.Entities;
using ogybot.Util;

namespace ogybot.DataAccess.Clients;

public class WaitlistClient
{
    private const string Endpoint = "waitlist";

    private readonly HttpClient _client;
    private readonly string _validationKey;

    public WaitlistClient(IConfiguration configuration)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(configuration["Api:Uri"]!)
        };

        _validationKey = configuration["Api:ValidationKey"]!;
    }

    /// <summary>
    /// Gets users queued on the wait list
    /// </summary>
    /// <returns>
    /// List of users in tome queue
    /// </returns>
    public async Task<List<UserWaitlist>> GetListAsync()
    {
        var listJson = await _client.GetAsync(Endpoint);

        var listOfUsers = JsonConvert
            .DeserializeObject<List<UserWaitlist>>(
                await listJson.Content.ReadAsStringAsync());

        return listOfUsers!;
    }

    /// <summary>
    /// Adds user to waitlist queue
    /// </summary>
    /// <param name="user">User to be added on the list</param>
    /// <returns>Response of type <see cref="Response"/></returns>
    public async Task<Response> PostUserAsync(UserWaitlist user)
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

        // Check for errors
        var errorMessage = ErrorMessages.AddUserToListError;

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(responseContent);

            if (errorResponse != null)
            {
                errorMessage = errorResponse.Error;
            }
        }


        // Return true if success status code, else return false and an error.
        return response.IsSuccessStatusCode
            ? new Response(user.Username!, true)
            : new Response(user.Username!, false, errorMessage);
    }

    /// <summary>
    /// Removes user from waitlist queue
    /// </summary>
    /// <param name="user">User to be removed</param>
    /// <returns>Response of type <see cref="Response"/></returns>
    public async Task<Response> RemoveUserAsync(UserWaitlist user)
    {
        // Get token, check if it's null and add to headers
        var token = await GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            return new Response("", false, ErrorMessages.GetTokenError);
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Make request
        var response = await _client.DeleteAsync($"{Endpoint}/{user.Username}");

        // Return true if success status code, else return false and an error.
        return response.IsSuccessStatusCode
            ? new Response(user.Username!, true)
            : new Response("", false, ErrorMessages.RemoveUserFromListError);
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
            validationKey = _validationKey
        });

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        // Get token response from API
        var response = await _client.PostAsync("auth/gettoken", content);

        if (!response.IsSuccessStatusCode) return null;

        var apiResponse = await response.Content.ReadFromJsonAsync<TokenApiResponse>();
        return apiResponse!.Token;
    }
}