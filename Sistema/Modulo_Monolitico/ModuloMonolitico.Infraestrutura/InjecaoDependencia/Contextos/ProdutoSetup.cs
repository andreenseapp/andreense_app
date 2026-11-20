using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Produto;

namespace Microsoft.Extensions.DependencyInjection;

public static class ProdutoSetup
{
    public static IServiceCollection AddContextoProduto(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<ProdutoRepositorio>(_ => new ProdutoRepositorio(connectionString));
        services.AddScoped<IProdutoRepositorio>(sp => sp.GetRequiredService<ProdutoRepositorio>());
        services.AddScoped<IProdutoConsultas>(sp => sp.GetRequiredService<ProdutoRepositorio>());

        return services;
    }
}
