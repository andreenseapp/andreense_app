using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Aplicacao.Interfaces;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.ObjetosDeValor.Base;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class RenovarTokenCommand : IRequest<ResultadoAutenticacaoDto>
{
    public string TokenRenovacao { get; set; } = string.Empty;
    public string AgenteUsuario { get; set; } = string.Empty;
    public string IdiomaAceito { get; set; } = string.Empty;
    public string Plataforma { get; set; } = string.Empty;
    public string EnderecoIp { get; set; } = string.Empty;
}

public class RenovarTokenCommandHandler : IRequestHandler<RenovarTokenCommand, ResultadoAutenticacaoDto>
{
    private readonly ISessaoRepositorio _sessaoRepo;
    private readonly IUsuarioRepositorio _usuarioRepo;
    private readonly IEmpresaRepositorio _empresaRepo;
    private readonly IServicoToken _servicoToken;
    private readonly IServicoImpressaoDigital _servicoImpressaoDigital;
    private readonly IAssinaturaSaaSQueries _assinaturaSaaSQueries;

    public RenovarTokenCommandHandler(
        ISessaoRepositorio sessaoRepo,
        IUsuarioRepositorio usuarioRepo,
        IEmpresaRepositorio empresaRepo,
        IServicoToken servicoToken,
        IServicoImpressaoDigital servicoImpressaoDigital,
        IAssinaturaSaaSQueries assinaturaSaaSQueries)
    {
        _sessaoRepo = sessaoRepo;
        _usuarioRepo = usuarioRepo;
        _empresaRepo = empresaRepo;
        _servicoToken = servicoToken;
        _servicoImpressaoDigital = servicoImpressaoDigital;
        _assinaturaSaaSQueries = assinaturaSaaSQueries;
    }

    public async Task<ResultadoAutenticacaoDto> Handle(RenovarTokenCommand comando, CancellationToken cancellationToken)
    {
        var hashTokenRenovacao = _servicoToken.GerarHashTokenRenovacao(comando.TokenRenovacao);
        var sessao = await _sessaoRepo.ObterPorHashRenovacaoAsync(hashTokenRenovacao);

        if (sessao == null || !sessao.Ativa || sessao.DataExpiracao < DateTimeOffset.UtcNow)
            return ResultadoAutenticacaoDto.Falha("Sessão inválida ou expirada. Faça login novamente.");

        var impressaoAtual = new InfoImpressaoDigital
        {
            AgenteUsuario = comando.AgenteUsuario,
            IdiomaAceito = comando.IdiomaAceito,
            Plataforma = comando.Plataforma,
            EnderecoIp = comando.EnderecoIp
        };
        var hashImpressaoAtual = _servicoImpressaoDigital.GerarHashImpressaoDigital(impressaoAtual);
        var impressaoSessao = new InfoImpressaoDigital
        {
            AgenteUsuario = sessao.AgenteUsuario ?? string.Empty,
            Plataforma = sessao.Plataforma ?? string.Empty,
            EnderecoIp = sessao.EnderecoIp ?? string.Empty
        };
        var pontuacao = impressaoAtual.CalcularScore(impressaoSessao);

        if (pontuacao < 60)
        {
            await _sessaoRepo.RevogarAsync(sessao.Id);
            return ResultadoAutenticacaoDto.Falha("Sessão revogada por segurança. Faça login novamente.");
        }

        var usuario = await _usuarioRepo.ObterPorIdAsync(sessao.UsuarioId);
        if (usuario == null || !usuario.Ativo)
            return ResultadoAutenticacaoDto.Falha("Usuário não encontrado ou inativo.");

        var empresas = await _empresaRepo.ListarTodasAcessiveisAoUsuarioAsync(usuario.Id, sessao.ContaId);
        var idsEmpresas = empresas.Select(e => e.Id).ToList();
        var novoTokenRenovacao = _servicoToken.GerarTokenRenovacao();
        var novoHashTokenRenovacao = _servicoToken.GerarHashTokenRenovacao(novoTokenRenovacao);
        await _sessaoRepo.RevogarAsync(sessao.Id);

        var novaSessao = new SessaoUsuario
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            ContaId = sessao.ContaId,
            HashTokenRenovacao = novoHashTokenRenovacao,
            HashImpressaoDigital = hashImpressaoAtual,
            EnderecoIp = comando.EnderecoIp,
            AgenteUsuario = comando.AgenteUsuario,
            Plataforma = comando.Plataforma,
            DataInclusao = DateTimeOffset.UtcNow,
            DataExpiracao = DateTimeOffset.UtcNow.AddDays(7),
            Ativa = true
        };
        await _sessaoRepo.CadastrarAsync(novaSessao);
        var direitosPlano = await _assinaturaSaaSQueries.ObterDireitosPlanoDaContaAsync(sessao.ContaId);

        var tokenAcesso = _servicoToken.GerarTokenAcesso(usuario, sessao.ContaId, idsEmpresas, hashImpressaoAtual, direitosPlano);

        return ResultadoAutenticacaoDto.ComSucesso(tokenAcesso, novoTokenRenovacao, "Token renovado com sucesso.");
    }
}
