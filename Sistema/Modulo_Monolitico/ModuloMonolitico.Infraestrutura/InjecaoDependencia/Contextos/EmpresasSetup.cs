using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Empresas;

namespace Microsoft.Extensions.DependencyInjection;

public static class EmpresasSetup
{
    public static IServiceCollection AddContextoEmpresas(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IEmpresaRepositorio>(_ => new EmpresaRepositorio(connectionString));
        
        return services;
    }
}


