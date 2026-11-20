using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Agendamentos;

namespace Microsoft.Extensions.DependencyInjection;

public static class AgendamentosSetup
{
    public static IServiceCollection AddContextoAgendamentos(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<AgendamentoRepositorio>(_ => new AgendamentoRepositorio(connectionString));
        services.AddScoped<IAgendamentoRepositorio>(sp => sp.GetRequiredService<AgendamentoRepositorio>());
        services.AddScoped<IAgendamentoQueries>(sp => sp.GetRequiredService<AgendamentoRepositorio>());

        return services;
    }
}



