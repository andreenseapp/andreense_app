using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarContasComAssinaturaQuery() : IRequest<IEnumerable<ContaResumoDto>>;

public class ListarContasComAssinaturaQueryHandler : IRequestHandler<ListarContasComAssinaturaQuery, IEnumerable<ContaResumoDto>>
{
    private readonly IAssinaturaSaaSQueries _queries;

    public ListarContasComAssinaturaQueryHandler(IAssinaturaSaaSQueries queries) => _queries = queries;

    public Task<IEnumerable<ContaResumoDto>> Handle(ListarContasComAssinaturaQuery comando, CancellationToken cancellationToken)
        => _queries.ListarContasComAssinaturaAsync();
}
