using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ReenviarConviteEquipeCommand(Guid RemetenteId, Guid EmpresaId, Guid UsuarioId) : IRequest<bool>;

public class ReenviarConviteEquipeCommandHandler : IRequestHandler<ReenviarConviteEquipeCommand, bool>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IEmpresaRepositorio _empresaRepositorio;
    private readonly IMediator _mediator;

    public ReenviarConviteEquipeCommandHandler(IUsuarioRepositorio usuarioRepositorio, IEmpresaRepositorio empresaRepositorio, IMediator mediator)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _empresaRepositorio = empresaRepositorio;
        _mediator = mediator;
    }

    public async Task<bool> Handle(ReenviarConviteEquipeCommand comando, CancellationToken cancellationToken)
    {
        var existeVinculo = await _usuarioRepositorio.ExisteVinculoAsync(comando.UsuarioId, comando.EmpresaId);
        if (!existeVinculo) return false;
        var empresa = await _empresaRepositorio.ObterPorIdAsync(comando.EmpresaId);
        if (empresa != null)
        {
            var corpo = string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Empresas.TextoConvite, empresa.Nome, comando.EmpresaId);
            await _mediator.Send(new ModuloMonolitico.Aplicacao.CasosDeUso.EnviarMensagemSistemaCommand(comando.UsuarioId, comando.RemetenteId, "Novo Convite de Equipe (Reenvio)", corpo), cancellationToken);
        }
        
        return true;
    }
}



