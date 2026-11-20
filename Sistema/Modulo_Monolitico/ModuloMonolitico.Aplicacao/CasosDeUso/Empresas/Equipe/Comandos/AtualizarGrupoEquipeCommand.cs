using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record AtualizarGrupoEquipeCommand(Guid EmpresaId, Guid UsuarioId, Guid NovoGrupoId) : IRequest<bool>;

public class AtualizarGrupoEquipeCommandHandler : IRequestHandler<AtualizarGrupoEquipeCommand, bool>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public AtualizarGrupoEquipeCommandHandler(IUsuarioRepositorio usuarioRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
    }

    public async Task<bool> Handle(AtualizarGrupoEquipeCommand comando, CancellationToken cancellationToken)
    {
        var existeVinculo = await _usuarioRepositorio.ExisteVinculoAsync(comando.UsuarioId, comando.EmpresaId);
        if (!existeVinculo) return false;

        await _usuarioRepositorio.AtualizarGrupoMembroAsync(comando.UsuarioId, comando.EmpresaId, comando.NovoGrupoId);
        return true;
    }
}



