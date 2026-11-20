using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record DeletarFisicoMembroEquipeCommand(Guid UsuarioId, Guid EmpresaId) : IRequest<bool>;

public class DeletarFisicoMembroEquipeCommandHandler : IRequestHandler<DeletarFisicoMembroEquipeCommand, bool>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public DeletarFisicoMembroEquipeCommandHandler(IUsuarioRepositorio usuarioRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
    }

    public async Task<bool> Handle(DeletarFisicoMembroEquipeCommand comando, CancellationToken cancellationToken)
    {
        await _usuarioRepositorio.DeletarFisicoMembroAsync(comando.UsuarioId, comando.EmpresaId);
        return true;
    }
}


