using Microsoft.Extensions.DependencyInjection;
using ogybot.Extensions;

namespace ogybot.Builders;

public class ServiceBuilder
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();
        services.AddServices();

        return services.BuildServiceProvider();
    }
}