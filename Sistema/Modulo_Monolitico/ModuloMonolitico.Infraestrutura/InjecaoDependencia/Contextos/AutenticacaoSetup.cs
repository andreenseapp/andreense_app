using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Autenticacao;
using ModuloMonolitico.Infraestrutura.Servicos.Autenticacao;

namespace Microsoft.Extensions.DependencyInjection;

public static class AutenticacaoSetup
{
    public static IServiceCollection AddContextoAutenticacao(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IUsuarioRepositorio>(sp =>
            new UsuarioRepositorio(
                connectionString,
                sp.GetRequiredService<ModuloMonolitico.Infraestrutura.AcessoADados.Contexto.DbContext>(),
                sp.GetService<Microsoft.Extensions.Logging.ILogger<UsuarioRepositorio>>()));
        services.AddScoped<ITentativaLoginRepositorio>(_ => new TentativaLoginRepositorio(connectionString));
        services.AddScoped<ISessaoRepositorio>(_ => new SessaoRepositorio(connectionString));
        services.AddScoped<ICodigoValidacaoRepositorio>(_ => new CodigoValidacaoRepositorio(connectionString));
        services.AddScoped<IGrupoUsuarioRepositorio>(_ => new GrupoUsuarioRepositorio(connectionString));
        services.AddSingleton<IServicoLimiteAcesso, ServicoLimiteAcesso>();

        return services;
    }
}


