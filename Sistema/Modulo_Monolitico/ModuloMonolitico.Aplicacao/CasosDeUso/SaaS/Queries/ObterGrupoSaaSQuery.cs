using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public record ObterGrupoSaaSQuery(Guid Id) : IRequest<GrupoUsuarioSaaS?>;

public class ObterGrupoSaaSQueryHandler : IRequestHandler<ObterGrupoSaaSQuery, GrupoUsuarioSaaS?>
{
    private readonly IGrupoUsuarioSaaSRepositorio _repo;
    public ObterGrupoSaaSQueryHandler(IGrupoUsuarioSaaSRepositorio repo) => _repo = repo;

    public async Task<GrupoUsuarioSaaS?> Handle(ObterGrupoSaaSQuery comando, CancellationToken cancellationToken)
        => await _repo.ObterPorIdAsync(comando.Id);
}



