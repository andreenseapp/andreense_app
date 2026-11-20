using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Fidelizacao;

namespace Microsoft.Extensions.DependencyInjection;

public static class FidelizacaoSetup
{
    public static IServiceCollection AddContextoFidelizacao(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<ICashbackRepositorio>(_ => new CashbackRepositorio(connectionString));
        services.AddScoped<IFidelidadeRepositorio>(_ => new FidelidadeRepositorio(connectionString));
        
        return services;
    }
}

