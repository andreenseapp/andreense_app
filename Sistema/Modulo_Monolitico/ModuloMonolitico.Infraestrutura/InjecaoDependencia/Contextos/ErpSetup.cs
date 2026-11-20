using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.ERP;

namespace Microsoft.Extensions.DependencyInjection;

public static class ErpSetup
{
    public static IServiceCollection AddContextoErp(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<ICategoriaRepositorio>(_ => new CategoriaRepositorio(connectionString));
        services.AddScoped<IMarcaRepositorio>(_ => new MarcaRepositorio(connectionString));
        services.AddScoped<IUnidadeMedidaRepositorio>(_ => new UnidadeMedidaRepositorio(connectionString));

        return services;
    }
}

