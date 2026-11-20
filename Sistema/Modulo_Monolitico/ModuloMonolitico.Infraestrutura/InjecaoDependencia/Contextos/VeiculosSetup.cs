using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Veiculos;

namespace Microsoft.Extensions.DependencyInjection;

public static class VeiculosSetup
{
    public static IServiceCollection AddContextoVeiculos(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IAnuncioVeiculoRepositorio>(_ => new AnuncioVeiculoRepositorio(connectionString));
        services.AddScoped<IPerfilVendedorRepositorio>(_ => new PerfilVendedorRepositorio(connectionString));
        services.AddScoped<IVeiculoFavoritoRepositorio>(_ => new VeiculoFavoritoRepositorio(connectionString));
        services.AddScoped<IBuscaRecenteVeiculoRepositorio>(_ => new BuscaRecenteVeiculoRepositorio(connectionString));
        return services;
    }
}
