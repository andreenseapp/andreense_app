using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.Entidades.Servicos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarPrestadoresQuery(Guid EmpresaId, Guid ContaId, bool ApenasAtivos = true, Guid? ProdutoVariacaoId = null) : IRequest<IEnumerable<Prestador>>;

public class ListarPrestadoresQueryHandler : IRequestHandler<ListarPrestadoresQuery, IEnumerable<Prestador>>
{
    private readonly IAgendamentoRepositorio _repo;
    public ListarPrestadoresQueryHandler(IAgendamentoRepositorio repo) => _repo = repo;

    public async Task<IEnumerable<Prestador>> Handle(ListarPrestadoresQuery q, CancellationToken ct)
        => await _repo.ListarPrestadoresAsync(q.EmpresaId, q.ContaId, q.ApenasAtivos, q.ProdutoVariacaoId);
}



