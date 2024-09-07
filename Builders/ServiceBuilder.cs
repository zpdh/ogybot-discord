using Microsoft.Extensions.DependencyInjection;
using ogybot.Extensions;

namespace ogybot.Builders;

public static class ServiceBuilder
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();
        services.AddServices();

        return services.BuildServiceProvider();
    }
}