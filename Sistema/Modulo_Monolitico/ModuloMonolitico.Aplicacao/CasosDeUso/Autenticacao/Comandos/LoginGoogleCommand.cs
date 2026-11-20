using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Aplicacao.Interfaces;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Aplicacao.Servicos;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.ObjetosDeValor.Base;
using ModuloMonolitico.Dominio.Servicos;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class LoginGoogleCommand : IRequest<ResultadoAutenticacaoDto>
{
    public string TokenIdentidade { get; set; } = string.Empty;
    public string AgenteUsuario { get; set; } = string.Empty;
    public string EnderecoIp { get; set; } = string.Empty;
}

public class LoginGoogleCommandHandler : IRequestHandler<LoginGoogleCommand, ResultadoAutenticacaoDto>
{
    private const string MsgFalhaGoogle = "Não foi possível entrar com Google. Tente novamente.";
    private const string EmailFalhaToken = "(google-token-invalido)";

    private readonly IServicoAutenticacaoGoogle _autenticacaoGoogle;
    private readonly IUsuarioRepositorio _usuarioRepo;
    private readonly IEmpresaRepositorio _empresaRepo;
    private readonly ISessaoRepositorio _sessaoRepo;
    private readonly IServicoLimitacaoRequisicoes _limitacaoRequisicoes;
    private readonly IServicoToken _servicoToken;
    private readonly IServicoImpressaoDigital _servicoImpressaoDigital;
    private readonly IAssinaturaSaaSQueries _assinaturaSaaSQueries;

    public LoginGoogleCommandHandler(
        IServicoAutenticacaoGoogle autenticacaoGoogle,
        IUsuarioRepositorio usuarioRepo,
        IEmpresaRepositorio empresaRepo,
        ISessaoRepositorio sessaoRepo,
        IServicoLimitacaoRequisicoes limitacaoRequisicoes,
        IServicoToken servicoToken,
        IServicoImpressaoDigital servicoImpressaoDigital,
        IAssinaturaSaaSQueries assinaturaSaaSQueries)
    {
        _autenticacaoGoogle = autenticacaoGoogle;
        _usuarioRepo = usuarioRepo;
        _empresaRepo = empresaRepo;
        _sessaoRepo = sessaoRepo;
        _limitacaoRequisicoes = limitacaoRequisicoes;
        _servicoToken = servicoToken;
        _servicoImpressaoDigital = servicoImpressaoDigital;
        _assinaturaSaaSQueries = assinaturaSaaSQueries;
    }

    public async Task<ResultadoAutenticacaoDto> Handle(LoginGoogleCommand comando, CancellationToken cancellationToken)
    {
        var carga = await _autenticacaoGoogle.ValidarTokenIdentidadeAsync(comando.TokenIdentidade);
        if (carga == null)
        {
            if (await _limitacaoRequisicoes.EstaBloqueadoAsync(
                    PoliticasLimitacaoRequisicoes.Login, EmailFalhaToken, comando.EnderecoIp, cancellationToken))
            {
                await _limitacaoRequisicoes.AplicarAtrasoRespostaAsync(cancellationToken);
                return ResultadoAutenticacaoDto.Falha(MsgFalhaGoogle);
            }

            await RegistrarFalhaGoogleAsync(EmailFalhaToken, comando, "token_invalido", cancellationToken);
            return ResultadoAutenticacaoDto.Falha(MsgFalhaGoogle);
        }

        if (await _limitacaoRequisicoes.EstaBloqueadoAsync(
                PoliticasLimitacaoRequisicoes.Login, carga.Email, comando.EnderecoIp, cancellationToken))
        {
            await _limitacaoRequisicoes.AplicarAtrasoRespostaAsync(cancellationToken);
            return ResultadoAutenticacaoDto.Falha(MsgFalhaGoogle);
        }

        var usuario = await _usuarioRepo.ObterPorEmailAsync(carga.Email);
        if (usuario == null)
        {
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString());

            usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = carga.Nome,
                Email = carga.Email,
                SenhaHash = senhaHash,
                Ativo = true,
                EmailValidado = true
            };

            var conta = new Conta { Id = Guid.NewGuid(), Nome = carga.Nome };

            var empresa = new Empresa
            {
                Id = Guid.NewGuid(),
                ContaId = conta.Id,
                Nome = carga.Nome,
                RazaoSocial = carga.Nome,
                Cnpj = "00000000000000",
                Ativo = true
            };

            await _usuarioRepo.CadastrarComContaEEmpresaAsync(usuario, conta, empresa);
        }

        if (!usuario.Ativo)
        {
            await RegistrarFalhaGoogleAsync(carga.Email, comando, "conta_indisponivel", cancellationToken);
            return ResultadoAutenticacaoDto.Falha(MsgFalhaGoogle);
        }

        var contaId = await _usuarioRepo.ObterContaIdPrincipalAsync(usuario.Id);
        if (contaId == null)
        {
            await RegistrarFalhaGoogleAsync(carga.Email, comando, "conta_indisponivel", cancellationToken);
            return ResultadoAutenticacaoDto.Falha(MsgFalhaGoogle);
        }

        var empresas = await _empresaRepo.ListarTodasAcessiveisAoUsuarioAsync(usuario.Id, contaId.Value);
        var idsEmpresas = empresas.Select(e => e.Id).ToList();

        await _limitacaoRequisicoes.RegistrarAsync(
            new RegistroTentativaLogin(carga.Email, comando.EnderecoIp, comando.AgenteUsuario, true, "google"),
            cancellationToken);

        await _sessaoRepo.RevogarTodasDoUsuarioAsync(usuario.Id);

        var hashImpressao = _servicoImpressaoDigital.GerarHashImpressaoDigital(new InfoImpressaoDigital
        {
            AgenteUsuario = comando.AgenteUsuario,
            EnderecoIp = comando.EnderecoIp
        });

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
            DataInclusao = DateTimeOffset.UtcNow,
            DataExpiracao = DateTimeOffset.UtcNow.AddDays(7),
            Ativa = true
        };

        await _sessaoRepo.CadastrarAsync(sessao);

        var direitosPlano = await _assinaturaSaaSQueries.ObterDireitosPlanoDaContaAsync(contaId.Value);
        var tokenAcesso = _servicoToken.GerarTokenAcesso(usuario, contaId.Value, idsEmpresas, hashImpressao, direitosPlano);

        return ResultadoAutenticacaoDto.ComSucesso(tokenAcesso, tokenRenovacao, "Login com Google realizado com sucesso.");
    }

    private async Task RegistrarFalhaGoogleAsync(
        string email,
        LoginGoogleCommand comando,
        string motivo,
        CancellationToken cancellationToken)
    {
        await _limitacaoRequisicoes.RegistrarAsync(
            new RegistroTentativaLogin(email, comando.EnderecoIp, comando.AgenteUsuario, false, "google", motivo),
            cancellationToken);
        await _limitacaoRequisicoes.AplicarAtrasoRespostaAsync(cancellationToken);
    }
}
