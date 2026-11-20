using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Base;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Geral;

namespace Microsoft.Extensions.DependencyInjection;

public static class GeralSetup
{
    public static IServiceCollection AddContextoGeral(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IAuditoriaRepositorio>(_ => new AuditoriaRepositorio(connectionString));
        services.AddScoped<IMensagensRepositorio>(_ => new MensagensRepositorio(connectionString));
        services.AddScoped<INotificacaoRepositorio>(_ => new NotificacaoRepositorio(connectionString));
        services.AddScoped<IPreferenciaTelaUsuarioRepositorio>(_ => new PreferenciaTelaUsuarioRepositorio(connectionString));
        services.AddScoped<IPreferenciaColunasTabelaRepositorio>(_ => new PreferenciaColunasTabelaRepositorio(connectionString));

        return services;
    }
}


