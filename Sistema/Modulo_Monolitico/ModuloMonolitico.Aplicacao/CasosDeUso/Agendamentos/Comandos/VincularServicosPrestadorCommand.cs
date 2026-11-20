using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record VincularServicosPrestadorCommand(
    Guid EmpresaId,
    Guid? UsuarioId,
    Guid? ContatoId,
    List<Guid> ProdutoVariacaoIds
) : IRequest<bool>;

public class VincularServicosPrestadorCommandHandler : IRequestHandler<VincularServicosPrestadorCommand, bool>
{
    private readonly IAgendamentoRepositorio _repo;
    public VincularServicosPrestadorCommandHandler(IAgendamentoRepositorio repo) => _repo = repo;

    public async Task<bool> Handle(VincularServicosPrestadorCommand cmd, CancellationToken ct)
    {
        await _repo.VincularServicosAoPrestadorAsync(cmd.EmpresaId, cmd.UsuarioId, cmd.ContatoId, cmd.ProdutoVariacaoIds);
        return true;
    }
}



