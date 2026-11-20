using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.SaaS.Comunicados;

public class DetalhesModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<DetalhesModel> _logger;

    public DetalhesModel(IMediator mediator, ILogger<DetalhesModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override bool ExigeEmpresaAtiva => false;

    public ComunicadoSaaS? Comunicado { get; private set; }
    public IEnumerable<InteracaoComunicadoDto> Interacoes { get; private set; } = [];
    public IEnumerable<ResultadoEnqueteDto> ResultadosEnquete { get; private set; } = [];

    public int TotalLidos => Interacoes.Count();
    public int TotalRespostas => Interacoes.Count(i => !string.IsNullOrEmpty(i.OpcaoEscolhida));

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado aos detalhes do comunicado SaaS para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            Comunicado = await _mediator.Send(new ObterComunicadoSaaSQuery(id));
            if (Comunicado == null)
            {
                _logger.LogWarning("Comunicado SaaS ID '{ComunicadoId}' não encontrado pelo usuário Master '{Email}'.", id, UsuarioEmail);
                MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Comunicados.NaoEncontrado;
                return RedirectToPage("Index");
            }

            Interacoes = await _mediator.Send(new ObterInteracoesComunicadoQuery(id));

            if (Comunicado.Tipo == "enquete")
            {
                ResultadosEnquete = await _mediator.Send(new ObterResultadosEnqueteComunicadoQuery(id));
            }

            _logger.LogInformation(
                "Detalhes do comunicado SaaS ID '{ComunicadoId}' consultados por '{Email}'. Leituras: {TotalLidos}, Respostas: {TotalRespostas}.",
                id,
                UsuarioEmail,
                TotalLidos,
                TotalRespostas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao carregar detalhes do comunicado SaaS ID '{ComunicadoId}' para o usuário '{Email}'.", id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Comunicados.ErroCarregarDetalhes;
            return RedirectToPage("Index");
        }

        return Page();
    }
}
