using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Gestao;

namespace Microsoft.Extensions.DependencyInjection;

public static class GestaoSetup
{
    public static IServiceCollection AddContextoGestao(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<ITipoVendedorRepositorio>(_ => new TipoVendedorRepositorio(connectionString));
        services.AddScoped<IVendedorRepositorio>(_ => new VendedorRepositorio(connectionString));
        return services;
    }
}


