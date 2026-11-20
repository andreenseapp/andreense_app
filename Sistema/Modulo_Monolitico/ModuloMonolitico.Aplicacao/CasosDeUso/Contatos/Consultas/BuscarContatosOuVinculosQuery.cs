using MediatR;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Aplicacao.DTOs;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record BuscarContatosOuVinculosQuery(
    Guid ContaId,
    Guid EmpresaId,
    string Papel,
    string Modo,
    string? Termo
) : IRequest<IEnumerable<ResultadoSeletorUsuario>>;

public class BuscarContatosOuVinculosQueryHandler
    : IRequestHandler<BuscarContatosOuVinculosQuery, IEnumerable<ResultadoSeletorUsuario>>
{
    private readonly IVinculoQueries _vinculoQueries;

    public BuscarContatosOuVinculosQueryHandler(IVinculoQueries vinculoQueries)
        => _vinculoQueries = vinculoQueries;

    public async Task<IEnumerable<ResultadoSeletorUsuario>> Handle(
        BuscarContatosOuVinculosQuery comando, CancellationToken cancellationToken)
    {
        if (comando.Modo == "vinculos")
            return await _vinculoQueries.BuscarContatosVinculadosAsync(
                comando.ContaId, comando.Papel, comando.Termo);

        return await _vinculoQueries.BuscarContatosInternosAsync(
            comando.ContaId, comando.EmpresaId, comando.Papel, comando.Termo);
    }
}
