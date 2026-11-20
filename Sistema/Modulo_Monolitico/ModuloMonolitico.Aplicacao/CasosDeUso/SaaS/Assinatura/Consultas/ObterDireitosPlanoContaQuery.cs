using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterDireitosPlanoContaQuery(Guid ContaId) : IRequest<ContaDireitosPlanoDto>;

public class ObterDireitosPlanoContaQueryHandler : IRequestHandler<ObterDireitosPlanoContaQuery, ContaDireitosPlanoDto>
{
    private readonly IAssinaturaSaaSQueries _queries;

    public ObterDireitosPlanoContaQueryHandler(IAssinaturaSaaSQueries queries) => _queries = queries;

    public Task<ContaDireitosPlanoDto> Handle(ObterDireitosPlanoContaQuery comando, CancellationToken cancellationToken)
        => _queries.ObterDireitosPlanoDaContaAsync(comando.ContaId);
}
