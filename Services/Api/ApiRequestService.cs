using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace test.Services.Api;

public static class ApiRequestService
{
    static HttpClient client = new HttpClient();

    public static async Task<Uri> CreateUserAsync(User user)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/users", user);
        response.EnsureSuccessStatusCode();

        return response.Headers.Location;
    }

    public static async Task<User> GetUserAsync(string path)
    {
        User user = null;

        HttpResponseMessage response = await client.GetAsync(path);
        if (response.IsSuccessStatusCode)
        {
            user = await response.Content.ReadAsAsync<User>();
        }
        return user;
    }

    public static async Task<IEnumerable<User>> GetUserListAsync()
    {
        List<User> userList = null;

        HttpResponseMessage response = await client.GetAsync("api/users");
        if (response.IsSuccessStatusCode)
        {
            userList = await response.Content.ReadFromJsonAsync<List<User>>();
        }
        return userList;
    }

    public static async Task<HttpStatusCode> DeleteUserAsync(User user)
    {
        var userList = await GetUserListAsync();
        var userInDb = userList.FirstOrDefault(x => x.Name.ToUpper() == user.Name.ToUpper());
        HttpResponseMessage response = await client.DeleteAsync($"api/users/{userInDb.Id}");
        return response.StatusCode;
    }

    public static void RunAsync()
    {
        client.BaseAddress = new Uri("http://ogybot-api.railway.internal:8081/");

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
