using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.Documentos;

public class EditarModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<EditarModel> _logger;

    public EditarModel(IMediator mediator, ILogger<EditarModel> logger)
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

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado à edição de documento legal para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            var documento = await _mediator.Send(new ObterDocumentoLegalQuery(id));
            if (documento == null)
            {
                _logger.LogWarning("Documento legal ID '{DocumentoId}' não encontrado para o usuário Master '{Email}'.", id, UsuarioEmail);
                return ModuloRotas.Redirecionar(ModuloRotas.InfraAdmin.Documentos);
            }

            Formulario = new DocumentoFormulario
            {
                Id = documento.Id,
                Tipo = documento.Tipo,
                Versao = documento.Versao,
                Titulo = documento.Titulo ?? string.Empty,
                Texto = documento.Texto,
                Situacao = documento.Situacao
            };

            _logger.LogInformation(
                "Formulário de edição do documento legal '{Titulo}' (ID: {DocumentoId}) carregado por '{Email}'.",
                Formulario.Titulo,
                id,
                UsuarioEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao obter documento legal ID '{DocumentoId}' para o usuário Master '{Email}'.", id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Documentos.ErroCarregar;
            return ModuloRotas.Redirecionar(ModuloRotas.InfraAdmin.Documentos);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de atualizar documento legal pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        if (!ModelState.IsValid)
            return Page();

        try
        {
            var documento = await _mediator.Send(new ObterDocumentoLegalQuery(Formulario.Id));
            if (documento == null)
            {
                _logger.LogWarning("Documento legal ID '{DocumentoId}' não encontrado para atualização pelo usuário Master '{Email}'.", Formulario.Id, UsuarioEmail);
                MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Documentos.NaoEncontrado;
                return Page();
            }

            documento.Titulo = Formulario.Titulo;
            documento.Texto = Formulario.Texto;
            documento.Situacao = Formulario.Situacao;

            await _mediator.Send(new AtualizarDocumentoLegalCommand(documento));
            await RegistrarAuditoriaAsync("documentos_legais", documento.Id.ToString(), "editar", documento);
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation(
                "Documento legal '{Titulo}' (ID: '{DocumentoId}') atualizado com sucesso pelo usuário Master '{Email}'.",
                Formulario.Titulo,
                Formulario.Id,
                UsuarioEmail);
            return ModuloRotas.Redirecionar(ModuloRotas.InfraAdmin.Documentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao atualizar documento legal ID '{DocumentoId}' pelo usuário Master '{Email}'.", Formulario.Id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Documentos.ErroAtualizar;
            return Page();
        }
    }

    public class DocumentoFormulario
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Versao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Título é obrigatório.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Texto é obrigatório.")]
        public string Texto { get; set; } = string.Empty;

        [Required]
        public string Situacao { get; set; } = "ativo";
    }
}
