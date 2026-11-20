using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class HelpDeskSetup
{
    public static IServiceCollection AddContextoHelpDesk(this IServiceCollection services, string connectionString)
    {
        return services;
    }
}

