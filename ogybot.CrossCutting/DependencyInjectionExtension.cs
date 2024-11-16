using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ogybot.Communication.Constants;
using ogybot.Data.Clients;
using ogybot.Data.Security.Tokens;
using ogybot.Data.Sockets;
using ogybot.Domain.Clients;
using ogybot.Domain.Security;
using ogybot.Domain.Sockets;

namespace ogybot.CrossCutting;

public static class DependencyInjectionExtension
{
    public static void AddDependencies(this ServiceCollection services)
    {
        services.AddHttpClient();
        services.AddTokenRequester();
        services.AddCustomClients();
        services.AddWebSockets();
    }

    private static void AddHttpClient(this ServiceCollection services)
    {
        services.AddSingleton<HttpClient>(provider => {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var baseAddress = configuration.GetValue<Uri>("Api:Uri");

            return new HttpClient { BaseAddress = baseAddress };
        });
    }

    private static void AddTokenRequester(this ServiceCollection services)
    {

        services.AddSingleton<ITokenRequester>(provider => {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var validationKey = configuration.GetValue<string>("Api:ValidationKey")!;

            var httpClient = provider.GetRequiredService<HttpClient>();

            return new TokenRequester(httpClient, validationKey);
        });
    }

    private static void AddCustomClients(this ServiceCollection services)
    {
        services.AddScoped<ITomeListClient, TomeListClient>();
        services.AddScoped<IWaitListClient, WaitListClient>();
        services.AddScoped<IAspectListClient, AspectListClient>();
        services.AddScoped<IOnlineClient, OnlineClient>();
    }

    private static void AddWebSockets(this ServiceCollection services)
    {
        services.AddSingleton<IChatSocket>(provider => {
            var config = provider.GetRequiredService<IConfiguration>();
            var tokenRequester = provider.GetRequiredService<ITokenRequester>();
            var websocketUrl = config.GetValue<string>("Websocket:WebsocketServerUrl")!;

            return new ChatSocket(tokenRequester, websocketUrl);
        });
    }
}