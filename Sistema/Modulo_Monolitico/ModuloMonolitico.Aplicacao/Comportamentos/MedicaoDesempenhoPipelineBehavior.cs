using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using System.Diagnostics;

namespace ModuloMonolitico.Aplicacao.Comportamentos;

public class MedicaoDesempenhoPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IRegistradorDesempenhoRequisicao _registrador;

    public MedicaoDesempenhoPipelineBehavior(IRegistradorDesempenhoRequisicao registrador)
    {
        _registrador = registrador;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_registrador.EstaHabilitado)
            return await next();

        var nome = typeof(TRequest).Name;
        var cronometro = Stopwatch.StartNew();
        try
        {
            return await next();
        }
        finally
        {
            cronometro.Stop();
            _registrador.RegistrarEtapa($"mediatr:{nome}", cronometro.ElapsedMilliseconds);
        }
    }
}
