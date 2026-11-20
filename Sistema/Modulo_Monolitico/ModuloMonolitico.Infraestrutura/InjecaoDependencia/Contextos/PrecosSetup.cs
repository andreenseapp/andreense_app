using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Precos;

namespace Microsoft.Extensions.DependencyInjection;

public static class PrecosSetup
{
    public static IServiceCollection AddContextoPrecos(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IContextoPrecoRepositorio>(_ => new ContextoPrecoRepositorio(connectionString));
        
        return services;
    }
}


