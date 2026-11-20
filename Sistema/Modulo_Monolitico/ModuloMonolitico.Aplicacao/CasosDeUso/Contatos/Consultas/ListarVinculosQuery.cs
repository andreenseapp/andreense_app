using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarVinculosQuery(Guid ContaId) : IRequest<ListarVinculosResultado>;

public record ListarVinculosResultado(
    IEnumerable<VinculoConta> Ativos,
    IEnumerable<SolicitacaoVinculo> Enviadas,
    IEnumerable<SolicitacaoVinculo> Recebidas
);

public class ListarVinculosQueryHandler : IRequestHandler<ListarVinculosQuery, ListarVinculosResultado>
{
    private readonly IVinculoRepositorio _vinculoRepositorio;

    public ListarVinculosQueryHandler(IVinculoRepositorio vinculoRepositorio)
        => _vinculoRepositorio = vinculoRepositorio;

    public async Task<ListarVinculosResultado> Handle(ListarVinculosQuery comando, CancellationToken cancellationToken)
    {
        var ativos = await _vinculoRepositorio.ListarVinculosAtivosPorContaAsync(comando.ContaId);
        var enviadas = await _vinculoRepositorio.ListarSolicitacoesEnviadasAsync(comando.ContaId);
        var recebidas = await _vinculoRepositorio.ListarSolicitacoesRecebidasAsync(comando.ContaId);
        return new ListarVinculosResultado(ativos, enviadas, recebidas);
    }
}
