using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class VarejoSetup
{
    public static IServiceCollection AddContextoVarejo(this IServiceCollection services, string connectionString)
    {
        return services;
    }
}

