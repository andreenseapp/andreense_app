using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.SaaS;

namespace Microsoft.Extensions.DependencyInjection;

public static class SaaSSetup
{
    public static IServiceCollection AddContextoSaaS(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<AssinaturaSaaSRepositorio>(_ => new AssinaturaSaaSRepositorio(connectionString));
        services.AddScoped<IAssinaturaSaaSRepositorio>(sp => sp.GetRequiredService<AssinaturaSaaSRepositorio>());
        services.AddScoped<IAssinaturaSaaSQueries>(sp => sp.GetRequiredService<AssinaturaSaaSRepositorio>());

        services.AddScoped<IComunicadoSaaSRepositorio>(_ => new ComunicadoSaaSRepositorio(connectionString));
        services.AddScoped<IGrupoUsuarioSaaSRepositorio>(_ => new GrupoUsuarioSaaSRepositorio(connectionString));

        return services;
    }
}



