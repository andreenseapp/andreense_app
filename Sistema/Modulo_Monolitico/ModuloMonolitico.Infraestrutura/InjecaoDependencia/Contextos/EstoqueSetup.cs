using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Estoque;
namespace Microsoft.Extensions.DependencyInjection;

public static class EstoqueSetup
{
    public static IServiceCollection AddContextoEstoque(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IEstoqueRepositorio>(_ => new EstoqueRepositorio(connectionString));
        
        return services;
    }
}


