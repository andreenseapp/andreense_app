using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterGrupoQuery(Guid Id, Guid EmpresaId) : IRequest<GrupoUsuario?>;

public class ObterGrupoQueryHandler : IRequestHandler<ObterGrupoQuery, GrupoUsuario?>
{
    private readonly IGrupoUsuarioRepositorio _grupoRepo;

    public ObterGrupoQueryHandler(IGrupoUsuarioRepositorio grupoRepo)
    {
        _grupoRepo = grupoRepo;
    }

    public async Task<GrupoUsuario?> Handle(ObterGrupoQuery comando, CancellationToken cancellationToken)
        => await _grupoRepo.ObterPorIdEEmpresaAsync(comando.Id, comando.EmpresaId);
}
