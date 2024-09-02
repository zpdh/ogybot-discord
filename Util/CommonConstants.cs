using Microsoft.Extensions.Configuration;
using test.Builders;

namespace test.Util;

/// <summary>
/// Class used to store common constants used throughout the entire application
/// </summary>
public static class CommonConstants
{
    public static readonly Uri ApiUri = new Uri("https://ico-server.onrender.com/");

    public static readonly string ValidationKey;

    static CommonConstants()
    {
        var config = AppConfigurationBuilder.Build();

        ValidationKey = config.GetValue<string>("ValidationKey")!;
    }
}