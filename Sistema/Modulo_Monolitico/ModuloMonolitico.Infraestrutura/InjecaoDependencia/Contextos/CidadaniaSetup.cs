using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Cidadania;
using ModuloMonolitico.Infraestrutura.Servicos.Sistema;

namespace Microsoft.Extensions.DependencyInjection;

public static class CidadaniaSetup
{
    public static IServiceCollection AddContextoCidadania(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IAchadosPerdidosRepositorio>(_ => new AchadosPerdidosRepositorio(connectionString));
        services.AddSingleton<IServicoGeradorCodigoBarras, ServicoGeradorCodigoBarras>();
        return services;
    }
}

