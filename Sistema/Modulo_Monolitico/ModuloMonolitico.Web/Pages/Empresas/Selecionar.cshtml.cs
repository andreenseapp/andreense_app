using ModuloMonolitico.Web.Core.Constantes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.Interfaces;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.ObjetosDeValor.Base;
using ModuloMonolitico.Dominio.Servicos;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Pages.Shared;
using ModuloMonolitico.Web.Servicos;
using ModuloMonolitico.Aplicacao.CasosDeUso;

namespace ModuloMonolitico.Web.Pages.Empresas;

public class SelecionarModel : AppBasePageModel
{
    protected override bool ExigeEmpresaAtiva => false;

    private readonly IServicoToken _servicoToken;
    private readonly IServicoImpressaoDigital _servicoImpressaoDigital;
    private readonly IMediator _mediator;
    private readonly IServicoCookieAutenticacao _cookiesAutenticacao;

    public SelecionarModel(
        IServicoToken servicoToken,
        IServicoImpressaoDigital servicoImpressaoDigital,
        IMediator mediator,
        IServicoCookieAutenticacao cookiesAutenticacao)
    {
        _servicoToken           = servicoToken;
        _servicoImpressaoDigital     = servicoImpressaoDigital;
        _mediator               = mediator;
        _cookiesAutenticacao = cookiesAutenticacao;
    }

    public List<Guid> EmpresasNoCookie => EmpresasAutorizadas.ToList();
    public IEnumerable<MinhaEmpresaDto> EmpresasDetalhes { get; private set; } = [];

    [BindProperty]
    public Guid EmpresaIdSelecionada { get; set; }

    [BindProperty(SupportsGet = true, Name = ModuloRotas.ParametroUrlRetorno)]
    public string? UrlRetorno { get; set; }

    public async Task OnGetAsync()
    {
        EmpresasDetalhes = await _mediator.Send(new ListarEmpresasUsuarioQuery(UsuarioId));
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var empresasAtualizadas = await _mediator.Send(new ListarEmpresasUsuarioQuery(UsuarioId));
            var idsAutorizados = empresasAtualizadas.Select(e => e.EmpresaId).ToList();

            if (!idsAutorizados.Contains(EmpresaIdSelecionada))
            {
                Logger.LogWarning("Usuário {UsuarioId} tentou selecionar empresa {EmpresaId} não autorizada. Autorizados: [{Ids}]",
                    UsuarioId, EmpresaIdSelecionada, string.Join(", ", idsAutorizados));
                return Forbid();
            }

            var empresa = await _mediator.Send(new ObterEmpresaPorIdQuery(EmpresaIdSelecionada));
            if (empresa == null)
            {
                Logger.LogWarning("Empresa {EmpresaId} não encontrada ao tentar selecionar.", EmpresaIdSelecionada);
                return NotFound();
            }

            var direitosPlano = await _mediator.Send(new ObterDireitosPlanoContaQuery(empresa.ContaId));

            var impressaoDigital = _servicoImpressaoDigital.GerarHashImpressaoDigital(new InfoImpressaoDigital
            {
                AgenteUsuario = Request.Headers["User-Agent"].ToString(),
                EnderecoIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty
            });

            var usuarioDoToken = new Usuario
            {
                Id             = UsuarioId,
                Email          = UsuarioEmail,
                EmailValidado  = EmailValidado
            };

            var tokenGerado = _servicoToken.GerarTokenAcesso(
                usuarioDoToken,
                empresa.ContaId,
                idsAutorizados,
                impressaoDigital,
                direitosPlano);

            _cookiesAutenticacao.DefinirTokenAcesso(tokenGerado);
            _cookiesAutenticacao.DefinirEmpresaAtiva(EmpresaIdSelecionada, empresa.Nome);

            Logger.LogInformation("Usuário {UsuarioId} selecionou empresa {EmpresaId} ({Nome}).", UsuarioId, EmpresaIdSelecionada, empresa.Nome);

            return ModuloRotas.Redirecionar(ResolverUrlDestino());
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro crítico ao tentar selecionar a empresa {EmpresaId} para o usuário {UsuarioId}.", EmpresaIdSelecionada, UsuarioId);
            NotificarErro(MensagensSistema.Empresas.ErroSelecionar);
            return Page();
        }
    }

    private string ResolverUrlDestino()
    {
        if (string.IsNullOrWhiteSpace(UrlRetorno) || !Url.IsLocalUrl(UrlRetorno))
            return ModuloRotas.ERP.Index;

        if (UrlRetorno.StartsWith(ModuloRotas.Empresas.Empresas_Selecionar, StringComparison.OrdinalIgnoreCase))
            return ModuloRotas.ERP.Index;

        return UrlRetorno;
    }
}
