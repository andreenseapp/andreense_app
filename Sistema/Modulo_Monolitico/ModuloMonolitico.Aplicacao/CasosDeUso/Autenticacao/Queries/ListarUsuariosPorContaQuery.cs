using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public record ListarUsuariosPorContaQuery(Guid ContaId) : IRequest<IEnumerable<Usuario>>;

public class ListarUsuariosPorContaQueryHandler : IRequestHandler<ListarUsuariosPorContaQuery, IEnumerable<Usuario>>
{
    private readonly IUsuarioRepositorio _usuarioRepo;

    public ListarUsuariosPorContaQueryHandler(IUsuarioRepositorio usuarioRepo)
    {
        _usuarioRepo = usuarioRepo;
    }

    public async Task<IEnumerable<Usuario>> Handle(ListarUsuariosPorContaQuery comando, CancellationToken cancellationToken)
        => await _usuarioRepo.ListarPorContaIdAsync(comando.ContaId);
}



