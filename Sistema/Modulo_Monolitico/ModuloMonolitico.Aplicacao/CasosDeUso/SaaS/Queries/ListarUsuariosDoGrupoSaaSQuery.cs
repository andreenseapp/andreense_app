using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public record ListarUsuariosDoGrupoSaaSQuery(Guid GrupoId) : IRequest<IEnumerable<UsuarioGrupoSaaS>>;

public class ListarUsuariosDoGrupoSaaSQueryHandler : IRequestHandler<ListarUsuariosDoGrupoSaaSQuery, IEnumerable<UsuarioGrupoSaaS>>
{
    private readonly IGrupoUsuarioSaaSRepositorio _repo;
    public ListarUsuariosDoGrupoSaaSQueryHandler(IGrupoUsuarioSaaSRepositorio repo) => _repo = repo;

    public async Task<IEnumerable<UsuarioGrupoSaaS>> Handle(ListarUsuariosDoGrupoSaaSQuery comando, CancellationToken cancellationToken)
        => await _repo.ListarUsuariosDoGrupoAsync(comando.GrupoId);
}



