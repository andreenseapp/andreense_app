using ModuloMonolitico.Dominio.InterfacesDeServico;
using System.Diagnostics;

namespace ModuloMonolitico.Web.Extensions;

public static class RegistradorDesempenhoExtensions
{
    public static async Task<T> MedirAsync<T>(
        this IRegistradorDesempenhoRequisicao registrador,
        string nome,
        Func<Task<T>> acao)
    {
        if (!registrador.EstaHabilitado)
            return await acao();

        var cronometro = Stopwatch.StartNew();
        try
        {
            return await acao();
        }
        finally
        {
            cronometro.Stop();
            registrador.RegistrarEtapa(nome, cronometro.ElapsedMilliseconds);
        }
    }

    public static async Task MedirAsync(
        this IRegistradorDesempenhoRequisicao registrador,
        string nome,
        Func<Task> acao)
    {
        if (!registrador.EstaHabilitado)
        {
            await acao();
            return;
        }

        var cronometro = Stopwatch.StartNew();
        try
        {
            await acao();
        }
        finally
        {
            cronometro.Stop();
            registrador.RegistrarEtapa(nome, cronometro.ElapsedMilliseconds);
        }
    }
}
