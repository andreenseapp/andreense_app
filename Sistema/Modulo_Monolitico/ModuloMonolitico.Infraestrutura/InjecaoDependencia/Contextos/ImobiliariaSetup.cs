using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Imobiliaria;
using ModuloMonolitico.Infraestrutura.Servicos.Imagem;

namespace Microsoft.Extensions.DependencyInjection;

public static class ImobiliariaSetup
{
    public static IServiceCollection AddContextoImobiliaria(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IAnuncioRepositorio>(_ => new AnuncioRepositorio(connectionString));
        services.AddScoped<IPerfilAnuncianteRepositorio>(_ => new PerfilAnuncianteRepositorio(connectionString));
        services.AddScoped<IAmenidadeRepositorio>(sp => new AmenidadeRepositorio(connectionString, sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>()));
        services.AddScoped<IImovelFavoritoRepositorio, ImovelFavoritoRepositorio>();
        services.AddScoped<IAnuncioVisualizadoRepositorio, AnuncioVisualizadoRepositorio>();
        services.AddScoped<IBuscaRecenteRepositorio>(_ => new BuscaRecenteRepositorio(connectionString));
        services.AddSingleton<IServicoProcessamentoImagem, ServicoProcessamentoImagem>();
        return services;
    }
}
