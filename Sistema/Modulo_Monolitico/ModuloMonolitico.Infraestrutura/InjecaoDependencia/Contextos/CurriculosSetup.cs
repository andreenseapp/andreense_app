using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Curriculos;

namespace Microsoft.Extensions.DependencyInjection;

public static class CurriculosSetup
{
    public static IServiceCollection AddContextoCurriculos(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<ICurriculoRepositorio>(_ => new CurriculoRepositorio(connectionString));
        return services;
    }
}
