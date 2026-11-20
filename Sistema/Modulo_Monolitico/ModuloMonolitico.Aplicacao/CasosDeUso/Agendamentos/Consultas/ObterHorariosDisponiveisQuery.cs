using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Aplicacao.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterHorariosDisponiveisQuery(
    Guid EmpresaId,
    Guid ProdutoVariacaoId,
    DateTime Data,
    Guid? ProfissionalUsuarioId = null,
    Guid? ProfissionalContatoId = null
) : IRequest<IEnumerable<HorarioDisponivel>>;

public class ObterHorariosDisponiveisQueryHandler : IRequestHandler<ObterHorariosDisponiveisQuery, IEnumerable<HorarioDisponivel>>
{
    private readonly IAgendamentoQueries _repo;
    public ObterHorariosDisponiveisQueryHandler(IAgendamentoQueries repo) => _repo = repo;

    public async Task<IEnumerable<HorarioDisponivel>> Handle(ObterHorariosDisponiveisQuery q, CancellationToken ct)
        => await _repo.ObterHorariosDisponiveisAsync(q.EmpresaId, q.ProdutoVariacaoId, q.Data, q.ProfissionalUsuarioId, q.ProfissionalContatoId);
}


