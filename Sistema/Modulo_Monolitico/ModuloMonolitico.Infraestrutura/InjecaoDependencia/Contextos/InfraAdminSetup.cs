using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.InfraAdmin;

namespace Microsoft.Extensions.DependencyInjection;

public static class InfraAdminSetup
{
    public static IServiceCollection AddContextoInfraAdmin(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IDocumentoRepositorio>(_ => new DocumentoRepositorio(connectionString));
        return services;
    }
}
