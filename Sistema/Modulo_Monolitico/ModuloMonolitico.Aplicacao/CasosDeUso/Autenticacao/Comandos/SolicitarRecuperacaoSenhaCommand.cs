using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Aplicacao.Interfaces;
using ModuloMonolitico.Aplicacao.Servicos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.InterfacesDeServico;

using Microsoft.Extensions.Logging;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class SolicitarRecuperacaoSenhaCommand : IRequest<ResultadoDto>
{
    public string Email { get; set; } = string.Empty;
    public string EnderecoIp { get; set; } = string.Empty;
    public string AgenteUsuario { get; set; } = string.Empty;
}

public class SolicitarRecuperacaoSenhaCommandHandler : IRequestHandler<SolicitarRecuperacaoSenhaCommand, ResultadoDto>
{
    private const string MsgSucesso =
        "Código enviado! Verifique sua caixa de entrada (válido por 15 minutos).";

    private const string MsgEmailNaoEncontrado =
        "Não encontramos uma conta ativa com este e-mail.";

    private const string MsgLimitacaoRequisicoes =
        "Muitas tentativas. Aguarde alguns minutos e tente novamente.";

    private readonly IUsuarioRepositorio _usuarioRepo;
    private readonly ICodigoValidacaoRepositorio _codigoRepo;
    private readonly IServicoLimitacaoRequisicoes _limitacaoRequisicoes;
    private readonly IServicoEmail _servicoEmail;
    private readonly ILogger<SolicitarRecuperacaoSenhaCommandHandler> _logger;

    public SolicitarRecuperacaoSenhaCommandHandler(
        IUsuarioRepositorio usuarioRepo,
        ICodigoValidacaoRepositorio codigoRepo,
        IServicoLimitacaoRequisicoes limitacaoRequisicoes,
        IServicoEmail servicoEmail,
        ILogger<SolicitarRecuperacaoSenhaCommandHandler> logger)
    {
        _usuarioRepo = usuarioRepo;
        _codigoRepo = codigoRepo;
        _limitacaoRequisicoes = limitacaoRequisicoes;
        _servicoEmail = servicoEmail;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(SolicitarRecuperacaoSenhaCommand comando, CancellationToken cancellationToken)
    {
        var email = comando.Email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("RecuperacaoSenha: e-mail não informado.");
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.EmailNaoFornecido);
        }

        _logger.LogInformation("RecuperacaoSenha: solicitação recebida para '{Email}' (IP {Ip}).", email, comando.EnderecoIp);

        if (await _limitacaoRequisicoes.EstaBloqueadoAsync(
                PoliticasLimitacaoRequisicoes.RecuperacaoSolicitacao, email, comando.EnderecoIp, cancellationToken))
        {
            _logger.LogWarning(
                "RecuperacaoSenha: rate limit ativo para '{Email}' (IP {Ip}).",
                email,
                comando.EnderecoIp);
            await _limitacaoRequisicoes.AplicarAtrasoRespostaAsync(cancellationToken);
            return ResultadoDto.Falha(MsgLimitacaoRequisicoes);
        }

        await _limitacaoRequisicoes.RegistrarAsync(new RegistroTentativaLogin(
            email,
            comando.EnderecoIp,
            comando.AgenteUsuario,
            true,
            PoliticasLimitacaoRequisicoes.RecuperacaoSolicitacao.Provedor!),
            cancellationToken);

        var usuario = await _usuarioRepo.ObterPorEmailAsync(email);

        if (usuario == null)
        {
            _logger.LogWarning("RecuperacaoSenha: e-mail '{Email}' não cadastrado.", email);
            await _limitacaoRequisicoes.AplicarAtrasoRespostaAsync(cancellationToken);
            return ResultadoDto.Falha(MsgEmailNaoEncontrado);
        }

        if (!usuario.Ativo)
        {
            _logger.LogWarning("RecuperacaoSenha: conta inativa para '{Email}'.", email);
            await _limitacaoRequisicoes.AplicarAtrasoRespostaAsync(cancellationToken);
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.ContaInativa);
        }

        var codigo = Random.Shared.Next(100000, 999999).ToString();

        await _codigoRepo.CadastrarAsync(usuario.Email, codigo, "recuperacao_senha", DateTime.UtcNow.AddMinutes(15));

        _logger.LogInformation(
            "RecuperacaoSenha: código {Codigo} gerado para '{Email}' (válido 15 min).",
            codigo,
            usuario.Email);

        var assunto = "Recuperação de Senha - Andreense";
        var corpo = $"""
            <h2>Olá, {usuario.Nome}</h2>
            <p>Você solicitou a recuperação de senha da sua conta.</p>
            <p>Seu código de verificação é:</p>
            <h1 style='color: #4CAF50; letter-spacing: 5px;'>{codigo}</h1>
            <p>Este código é válido por 15 minutos.</p>
            <p>Se você não solicitou isso, pode ignorar este e-mail em segurança.</p>
            """;

        var envio = await _servicoEmail.EnviarEmailAsync(usuario.Email, assunto, corpo);

        if (!envio.Sucesso)
        {
            _logger.LogError(
                "RecuperacaoSenha: FALHA NO ENVIO SMTP [{Categoria}] — conta remetente '{Remetente}' pode estar com problema. " +
                "Destino='{Email}' Código gerado={Codigo}. Detalhe: {Motivo}",
                envio.CategoriaFalha ?? "Desconhecida",
                envio.Remetente ?? "(não configurado)",
                usuario.Email,
                codigo,
                envio.MotivoFalha ?? "sem detalhe");

            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.ProblemaMomento);
        }

        _logger.LogInformation(
            "RecuperacaoSenha: e-mail aceito pelo SMTP. Remetente='{Remetente}' Destino='{Email}' Código={Codigo}",
            envio.Remetente,
            usuario.Email,
            codigo);

        return ResultadoDto.ComSucesso(MsgSucesso);
    }
}

