using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.SaaS.Comunicados;

public class CadastrarModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<CadastrarModel> _logger;

    public CadastrarModel(IMediator mediator, ILogger<CadastrarModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override bool ExigeEmpresaAtiva => false;

    [BindProperty]
    public string Titulo { get; set; } = string.Empty;

    [BindProperty]
    public string Mensagem { get; set; } = string.Empty;

    [BindProperty]
    public string Tipo { get; set; } = "aviso";

    [BindProperty]
    public string? ModuloAlvo { get; set; } = "Todos";

    [BindProperty]
    public DateTime? DataExpiracao { get; set; }

    [BindProperty]
    public List<string> OpcoesTextos { get; set; } = [];

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public IActionResult OnGet()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado ao cadastro de comunicados SaaS para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        _logger.LogInformation("Formulário de cadastro de comunicado SaaS acessado por '{Email}'.", UsuarioEmail);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de cadastrar comunicado SaaS pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(Titulo) || string.IsNullOrWhiteSpace(Mensagem))
        {
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Comunicados.TituloMensagemObrigatorios;
            return Page();
        }

        try
        {
            var dataExpiracaoUtc = DataExpiracao.HasValue ? DataExpiracao.Value.ToUniversalTime() : (DateTime?)null;
            var comunicado = new ComunicadoSaaS(Titulo.Trim(), Mensagem.Trim(), Tipo, ModuloAlvo == "Todos" ? null : ModuloAlvo, dataExpiracaoUtc, UsuarioId);

            var listaOpcoes = new List<OpcaoComunicadoSaaS>();
            if (Tipo == "enquete" && OpcoesTextos != null)
            {
                var ordem = 1;
                foreach (var texto in OpcoesTextos)
                {
                    if (!string.IsNullOrWhiteSpace(texto))
                    {
                        listaOpcoes.Add(new OpcaoComunicadoSaaS(comunicado.Id, texto.Trim(), ordem++));
                    }
                }

                if (listaOpcoes.Count < 2)
                {
                    MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Comunicados.EnqueteMinimoOpcoes;
                    return Page();
                }
            }

            await _mediator.Send(new CadastrarComunicadoSaaSCommand(comunicado, listaOpcoes));
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation(
                "Comunicado/Enquete '{Titulo}' cadastrado com sucesso pelo usuário Master '{Email}'.",
                Titulo,
                UsuarioEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao cadastrar comunicado/enquete SaaS pelo usuário '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Comunicados.ErroCadastrar;
            return Page();
        }

        return RedirectToPage("Index");
    }
}
