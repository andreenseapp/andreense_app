using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ConvidarEquipeCommand(Guid RemetenteId, Guid EmpresaId, string Email, Guid GrupoId) : IRequest<ConvidarEquipeResultado>;

public record ConvidarEquipeResultado(bool Sucesso, string? Erro);

public class ConvidarEquipeCommandHandler : IRequestHandler<ConvidarEquipeCommand, ConvidarEquipeResultado>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IEmpresaRepositorio _empresaRepositorio;
    private readonly IMediator _mediator;

    public ConvidarEquipeCommandHandler(IUsuarioRepositorio usuarioRepositorio, IEmpresaRepositorio empresaRepositorio, IMediator mediator)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _empresaRepositorio = empresaRepositorio;
        _mediator = mediator;
    }

    public async Task<ConvidarEquipeResultado> Handle(ConvidarEquipeCommand comando, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepositorio.ObterPorEmailAsync(comando.Email);
        if (usuario == null)
        {
            return new ConvidarEquipeResultado(false, "Este e-mail ainda não possui conta na plataforma. Peça para o usuário se cadastrar primeiro.");
        }
        var jaExiste = await _usuarioRepositorio.ExisteVinculoAsync(usuario.Id, comando.EmpresaId);
        if (jaExiste)
        {
            return new ConvidarEquipeResultado(false, "Este usuário já faz parte da equipe ou já possui um convite pendente.");
        }
        await _usuarioRepositorio.ConvidarMembroAsync(usuario.Id, comando.EmpresaId, comando.GrupoId);
        var empresa = await _empresaRepositorio.ObterPorIdAsync(comando.EmpresaId);
        if (empresa != null)
        {
            var corpo = string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Empresas.TextoConvite, empresa.Nome, comando.EmpresaId);
            await _mediator.Send(new ModuloMonolitico.Aplicacao.CasosDeUso.EnviarMensagemSistemaCommand(usuario.Id, comando.RemetenteId, "Novo Convite de Equipe", corpo), cancellationToken);
        }

        return new ConvidarEquipeResultado(true, null);
    }
}



