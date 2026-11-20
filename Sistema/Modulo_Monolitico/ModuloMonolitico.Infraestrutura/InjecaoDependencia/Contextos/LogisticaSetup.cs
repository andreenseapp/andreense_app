using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class LogisticaSetup
{
    public static IServiceCollection AddContextoLogistica(this IServiceCollection services, string connectionString)
    {
        return services;
    }
}

