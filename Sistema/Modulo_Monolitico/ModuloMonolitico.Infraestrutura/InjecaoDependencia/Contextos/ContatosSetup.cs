using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Contatos;

namespace Microsoft.Extensions.DependencyInjection;

public static class ContatosSetup
{
    public static IServiceCollection AddContextoContatos(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IContatoRepositorio>(_ => new ContatoRepositorio(connectionString));
        services.AddScoped<VinculoRepositorio>(_ => new VinculoRepositorio(connectionString));
        services.AddScoped<IVinculoRepositorio>(sp => sp.GetRequiredService<VinculoRepositorio>());
        services.AddScoped<IVinculoQueries>(sp => sp.GetRequiredService<VinculoRepositorio>());

        return services;
    }
}
