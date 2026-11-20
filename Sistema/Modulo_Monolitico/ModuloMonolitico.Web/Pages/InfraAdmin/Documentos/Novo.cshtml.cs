using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.Documentos;

public class NovoModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<NovoModel> _logger;

    public NovoModel(IMediator mediator, ILogger<NovoModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override bool ExigeEmpresaAtiva => false;

    [TempData]
    public string? MensagemErro { get; set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    [BindProperty]
    public DocumentoFormulario Formulario { get; set; } = new();

    public IActionResult OnGet()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado à criação de documento legal para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de criar documento legal pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        if (!ModelState.IsValid)
            return Page();

        try
        {
            var documento = new DocumentoLegal
            {
                Tipo = Formulario.Tipo,
                Versao = Formulario.Versao,
                Titulo = Formulario.Titulo,
                Texto = Formulario.Texto,
                Situacao = Formulario.Situacao
            };

            await _mediator.Send(new CadastrarDocumentoLegalCommand(documento));
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation("Documento legal '{Titulo}' criado com sucesso pelo usuário Master '{Email}'.", Formulario.Titulo, UsuarioEmail);
            return ModuloRotas.Redirecionar(ModuloRotas.InfraAdmin.Documentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao criar documento legal pelo usuário Master '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Documentos.ErroCriar;
            return Page();
        }
    }

    public class DocumentoFormulario
    {
        [Required(ErrorMessage = "O Tipo é obrigatório (ex: termo_de_uso).")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Versão é obrigatória (ex: 1.0).")]
        public string Versao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Título é obrigatório.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Texto é obrigatório.")]
        public string Texto { get; set; } = string.Empty;

        [Required]
        public string Situacao { get; set; } = "ativo";
    }
}
