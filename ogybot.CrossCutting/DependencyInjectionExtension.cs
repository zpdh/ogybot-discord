using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ogybot.Data.Security.Tokens;
using ogybot.Domain.Security;

namespace ogybot.CrossCutting;

public static class DependencyInjectionExtension
{
    public static void AddDependencies(this ServiceCollection services)
    {
        services.AddHttpClient();
        services.AddTokenRequester();
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
}