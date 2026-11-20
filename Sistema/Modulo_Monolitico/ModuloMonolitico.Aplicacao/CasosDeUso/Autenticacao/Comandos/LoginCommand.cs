using FluentValidation;
using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Aplicacao.Interfaces;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Aplicacao.Servicos;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.ObjetosDeValor.Base;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class LoginCommand : IRequest<ResultadoAutenticacaoDto>
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string AgenteUsuario { get; set; } = string.Empty;
    public string IdiomaAceito { get; set; } = string.Empty;
    public string Plataforma { get; set; } = string.Empty;
    public string EnderecoIp { get; set; } = string.Empty;
}

public class LoginCommandValidador : AbstractValidator<LoginCommand>
{
    public LoginCommandValidador()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail informado não é válido.");
        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.");
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, ResultadoAutenticacaoDto>
{
    private const string MsgCredenciaisInvalidas = "Credenciais inválidas. Verifique seu e-mail e senha.";

    private readonly IUsuarioRepositorio _usuarioRepo;
    private readonly ISessaoRepositorio _sessaoRepo;
    private readonly IServicoLimitacaoRequisicoes _limitacaoRequisicoes;
    private readonly IServicoToken _servicoToken;
    private readonly IServicoImpressaoDigital _servicoImpressaoDigital;
    private readonly IEmpresaRepositorio _empresaRepo;
    private readonly IAssinaturaSaaSQueries _assinaturaSaaSQueries;

    public LoginCommandHandler(
        IUsuarioRepositorio usuarioRepo,
        ISessaoRepositorio sessaoRepo,
        IServicoLimitacaoRequisicoes limitacaoRequisicoes,
        IServicoToken servicoToken,
        IServicoImpressaoDigital servicoImpressaoDigital,
        IEmpresaRepositorio empresaRepo,
        IAssinaturaSaaSQueries assinaturaSaaSQueries)
    {
        _usuarioRepo = usuarioRepo;
        _sessaoRepo = sessaoRepo;
        _limitacaoRequisicoes = limitacaoRequisicoes;
        _servicoToken = servicoToken;
        _servicoImpressaoDigital = servicoImpressaoDigital;
        _empresaRepo = empresaRepo;
        _assinaturaSaaSQueries = assinaturaSaaSQueries;
    }

    public async Task<ResultadoAutenticacaoDto> Handle(LoginCommand comando, CancellationToken cancellationToken)
    {
        if (await _limitacaoRequisicoes.EstaBloqueadoAsync(
                PoliticasLimitacaoRequisicoes.Login, comando.Email, comando.EnderecoIp, cancellationToken))
        {
            await _limitacaoRequisicoes.AplicarAtrasoRespostaAsync(cancellationToken);
            return ResultadoAutenticacaoDto.Falha(MsgCredenciaisInvalidas);
        }

        var usuario = await _usuarioRepo.ObterPorEmailAsync(comando.Email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(comando.Senha, usuario.SenhaHash))
        {
            await RegistrarFalhaAsync(comando, "credenciais_invalidas", cancellationToken);
            return ResultadoAutenticacaoDto.Falha(MsgCredenciaisInvalidas);
        }

        if (!usuario.Ativo)
        {
            await RegistrarFalhaAsync(comando, "conta_indisponivel", cancellationToken);
            return ResultadoAutenticacaoDto.Falha(MsgCredenciaisInvalidas);
        }

        var contaId = await _usuarioRepo.ObterContaIdPrincipalAsync(usuario.Id);
        if (contaId == null)
        {
            await RegistrarFalhaAsync(comando, "conta_indisponivel", cancellationToken);
            return ResultadoAutenticacaoDto.Falha(MsgCredenciaisInvalidas);
        }

        var empresas = await _empresaRepo.ListarTodasAcessiveisAoUsuarioAsync(usuario.Id, contaId.Value);
        var idsEmpresas = empresas.Select(e => e.Id).ToList();

        await _limitacaoRequisicoes.RegistrarAsync(
            new RegistroTentativaLogin(comando.Email, comando.EnderecoIp, comando.AgenteUsuario, true),
            cancellationToken);

        await _sessaoRepo.RevogarTodasDoUsuarioAsync(usuario.Id);

        var infoImpressao = new InfoImpressaoDigital
        {
            AgenteUsuario = comando.AgenteUsuario,
            IdiomaAceito = comando.IdiomaAceito,
            Plataforma = comando.Plataforma,
            EnderecoIp = comando.EnderecoIp
        };
        var hashImpressao = _servicoImpressaoDigital.GerarHashImpressaoDigital(infoImpressao);

        var tokenRenovacao = _servicoToken.GerarTokenRenovacao();
        var hashTokenRenovacao = _servicoToken.GerarHashTokenRenovacao(tokenRenovacao);

        var sessao = new SessaoUsuario
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            ContaId = contaId.Value,
            HashTokenRenovacao = hashTokenRenovacao,
            HashImpressaoDigital = hashImpressao,
            EnderecoIp = comando.EnderecoIp,
            AgenteUsuario = comando.AgenteUsuario,
            Plataforma = comando.Plataforma,
            DataInclusao = DateTimeOffset.UtcNow,
            DataExpiracao = DateTimeOffset.UtcNow.AddDays(7),
            Ativa = true
        };

        await _sessaoRepo.CadastrarAsync(sessao);

        var direitosPlano = await _assinaturaSaaSQueries.ObterDireitosPlanoDaContaAsync(contaId.Value);
        var tokenAcesso = _servicoToken.GerarTokenAcesso(usuario, contaId.Value, idsEmpresas, hashImpressao, direitosPlano);

        return ResultadoAutenticacaoDto.ComSucesso(tokenAcesso, tokenRenovacao, "Login realizado com sucesso.");
    }

    private async Task RegistrarFalhaAsync(LoginCommand comando, string motivo, CancellationToken cancellationToken)
    {
        await _limitacaoRequisicoes.RegistrarAsync(
            new RegistroTentativaLogin(comando.Email, comando.EnderecoIp, comando.AgenteUsuario, false, "local", motivo),
            cancellationToken);
        await _limitacaoRequisicoes.AplicarAtrasoRespostaAsync(cancellationToken);
    }
}
