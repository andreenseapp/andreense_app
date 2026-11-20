using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Aplicacao.Interfaces;
using ModuloMonolitico.Aplicacao.Servicos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class RedefinirSenhaCommand : IRequest<ResultadoDto>
{
    public string Email { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string NovaSenha { get; set; } = string.Empty;
    public string ConfirmarNovaSenha { get; set; } = string.Empty;
    public string EnderecoIp { get; set; } = string.Empty;
    public string AgenteUsuario { get; set; } = string.Empty;
}

public class RedefinirSenhaCommandHandler : IRequestHandler<RedefinirSenhaCommand, ResultadoDto>
{
    private const string MsgCodigoInvalido = "Código inválido ou expirado. Solicite um novo código.";

    private readonly IUsuarioRepositorio _usuarioRepo;
    private readonly ICodigoValidacaoRepositorio _codigoRepo;
    private readonly ISessaoRepositorio _sessaoRepo;
    private readonly IServicoLimitacaoRequisicoes _limitacaoRequisicoes;

    public RedefinirSenhaCommandHandler(
        IUsuarioRepositorio usuarioRepo,
        ICodigoValidacaoRepositorio codigoRepo,
        ISessaoRepositorio sessaoRepo,
        IServicoLimitacaoRequisicoes limitacaoRequisicoes)
    {
        _usuarioRepo = usuarioRepo;
        _codigoRepo = codigoRepo;
        _sessaoRepo = sessaoRepo;
        _limitacaoRequisicoes = limitacaoRequisicoes;
    }

    public async Task<ResultadoDto> Handle(RedefinirSenhaCommand comando, CancellationToken cancellationToken)
    {
        if (comando.NovaSenha != comando.ConfirmarNovaSenha)
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.SenhasNaoCoincidem);

        if (string.IsNullOrWhiteSpace(comando.Email) || string.IsNullOrWhiteSpace(comando.Codigo))
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.EmailECodigoObrigatorios);

        if (await _limitacaoRequisicoes.EstaBloqueadoAsync(
                PoliticasLimitacaoRequisicoes.RecuperacaoValidacao, comando.Email, comando.EnderecoIp, cancellationToken))
        {
            await _limitacaoRequisicoes.AplicarAtrasoRespostaAsync(cancellationToken);
            return ResultadoDto.Falha(MsgCodigoInvalido);
        }

        var usuario = await _usuarioRepo.ObterPorEmailAsync(comando.Email);
        if (usuario == null || !usuario.Ativo)
        {
            await RegistrarFalhaValidacaoAsync(comando, "conta_indisponivel", cancellationToken);
            return ResultadoDto.Falha(MsgCodigoInvalido);
        }

        var codigoValido = await _codigoRepo.ValidarAsync(usuario.Email, comando.Codigo, "recuperacao_senha");

        if (!codigoValido)
        {
            await RegistrarFalhaValidacaoAsync(comando, "codigo_invalido", cancellationToken);
            return ResultadoDto.Falha(MsgCodigoInvalido);
        }

        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(comando.NovaSenha);

        await _usuarioRepo.AtualizarAsync(usuario);
        await _sessaoRepo.RevogarTodasDoUsuarioAsync(usuario.Id);

        return ResultadoDto.ComSucesso("Senha redefinida com sucesso! Você já pode fazer o login.");
    }

    private async Task RegistrarFalhaValidacaoAsync(
        RedefinirSenhaCommand comando,
        string motivo,
        CancellationToken cancellationToken)
    {
        await _limitacaoRequisicoes.RegistrarAsync(new RegistroTentativaLogin(
            comando.Email,
            comando.EnderecoIp,
            comando.AgenteUsuario,
            false,
            PoliticasLimitacaoRequisicoes.RecuperacaoValidacao.Provedor!,
            motivo),
            cancellationToken);
        await _limitacaoRequisicoes.AplicarAtrasoRespostaAsync(cancellationToken);
    }
}


