using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Fiscal;

namespace Microsoft.Extensions.DependencyInjection;

public static class FiscalSetup
{
    public static IServiceCollection AddContextoFiscal(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IFiscalRepositorio>(_ => new FiscalRepositorio(connectionString));
        
        return services;
    }
}


