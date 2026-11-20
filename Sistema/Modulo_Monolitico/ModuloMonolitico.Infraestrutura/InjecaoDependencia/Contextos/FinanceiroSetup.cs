using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class FinanceiroSetup
{
    public static IServiceCollection AddContextoFinanceiro(this IServiceCollection services, string connectionString)
    {
        return services;
    }
}

