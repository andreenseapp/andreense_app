using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record AtualizarNomeUsuarioCommand(Guid UsuarioId, string Nome) : IRequest;

public class AtualizarNomeUsuarioCommandHandler : IRequestHandler<AtualizarNomeUsuarioCommand>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public AtualizarNomeUsuarioCommandHandler(IUsuarioRepositorio usuarioRepositorio)
        => _usuarioRepositorio = usuarioRepositorio;

    public async Task Handle(AtualizarNomeUsuarioCommand comando, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepositorio.ObterPorIdAsync(comando.UsuarioId)
            ?? throw new InvalidOperationException("Usuário não encontrado.");
        usuario.Nome = comando.Nome;
        await _usuarioRepositorio.AtualizarAsync(usuario);
    }
}

public record AtualizarSenhaUsuarioCommand(Guid UsuarioId, string SenhaAtual, string NovaSenha) : IRequest<(bool Sucesso, string? Erro)>;

public class AtualizarSenhaUsuarioCommandHandler : IRequestHandler<AtualizarSenhaUsuarioCommand, (bool, string?)>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public AtualizarSenhaUsuarioCommandHandler(IUsuarioRepositorio usuarioRepositorio)
        => _usuarioRepositorio = usuarioRepositorio;

    public async Task<(bool, string?)> Handle(AtualizarSenhaUsuarioCommand comando, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepositorio.ObterPorIdAsync(comando.UsuarioId)
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        if (!BCrypt.Net.BCrypt.Verify(comando.SenhaAtual, usuario.SenhaHash))
            return (false, "A senha atual informada está incorreta.");

        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(comando.NovaSenha);
        await _usuarioRepositorio.AtualizarAsync(usuario);
        return (true, null);
    }
}

public record CadastrarProfissaoUsuarioCommand(Guid UsuarioId, string Nome) : IRequest;

public class CadastrarProfissaoUsuarioCommandHandler : IRequestHandler<CadastrarProfissaoUsuarioCommand>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public CadastrarProfissaoUsuarioCommandHandler(IUsuarioRepositorio usuarioRepositorio)
        => _usuarioRepositorio = usuarioRepositorio;

    public Task Handle(CadastrarProfissaoUsuarioCommand comando, CancellationToken cancellationToken)
        => _usuarioRepositorio.CadastrarProfissaoAsync(new UsuarioProfissao
        {
            Id = Guid.NewGuid(),
            UsuarioId = comando.UsuarioId,
            Nome = comando.Nome.Trim()
        });
}

public record DeletarFisicoProfissaoUsuarioCommand(Guid ProfissaoId, Guid UsuarioId) : IRequest;

public class DeletarFisicoProfissaoUsuarioCommandHandler : IRequestHandler<DeletarFisicoProfissaoUsuarioCommand>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public DeletarFisicoProfissaoUsuarioCommandHandler(IUsuarioRepositorio usuarioRepositorio)
        => _usuarioRepositorio = usuarioRepositorio;

    public Task Handle(DeletarFisicoProfissaoUsuarioCommand comando, CancellationToken cancellationToken)
        => _usuarioRepositorio.DeletarFisicoProfissaoAsync(comando.ProfissaoId, comando.UsuarioId);
}

public record InativarUsuarioCommand(Guid UsuarioId) : IRequest;

public class InativarUsuarioCommandHandler : IRequestHandler<InativarUsuarioCommand>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public InativarUsuarioCommandHandler(IUsuarioRepositorio usuarioRepositorio)
        => _usuarioRepositorio = usuarioRepositorio;

    public Task Handle(InativarUsuarioCommand comando, CancellationToken cancellationToken)
        => _usuarioRepositorio.InativarAsync(comando.UsuarioId);
}

public record RevogarAcessoUsuarioCommand(Guid UsuarioId) : IRequest;

public class RevogarAcessoUsuarioCommandHandler : IRequestHandler<RevogarAcessoUsuarioCommand>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly ISessaoRepositorio _sessaoRepositorio;

    public RevogarAcessoUsuarioCommandHandler(IUsuarioRepositorio usuarioRepositorio, ISessaoRepositorio sessaoRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _sessaoRepositorio = sessaoRepositorio;
    }

    public async Task Handle(RevogarAcessoUsuarioCommand comando, CancellationToken cancellationToken)
    {
        await _usuarioRepositorio.InativarAsync(comando.UsuarioId);
        await _sessaoRepositorio.RevogarTodasDoUsuarioAsync(comando.UsuarioId);
    }
}
