using Microsoft.Extensions.Configuration;

namespace ogybot.Builders;

/// <summary>
/// Class responsible for <see cref="IConfiguration"/> generation.
/// </summary>
public static class AppConfigurationBuilder
{
    public static IConfiguration Build()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");

        return builder.Build();
    }

    public static string GetBranch(this IConfiguration configuration)
    {
        var sect = configuration.GetValue<string>("Branch");

        return sect == "Main" ? "Main" : "Dev";
    }
}