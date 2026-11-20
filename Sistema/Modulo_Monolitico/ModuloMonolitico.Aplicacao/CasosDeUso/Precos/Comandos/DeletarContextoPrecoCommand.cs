using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class DeletarContextoPrecoCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
}

public class DeletarContextoPrecoCommandHandler : IRequestHandler<DeletarContextoPrecoCommand>
{
    private readonly IContextoPrecoRepositorio _repo;

    public DeletarContextoPrecoCommandHandler(IContextoPrecoRepositorio repo) => _repo = repo;

    public async Task Handle(DeletarContextoPrecoCommand comando, CancellationToken cancellationToken)
        => await _repo.DeletarSeguroAsync(comando.Id, comando.ContaId);
}




