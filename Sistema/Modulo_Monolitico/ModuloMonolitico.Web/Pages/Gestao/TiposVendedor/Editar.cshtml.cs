using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Web.Pages.Shared;
using System.ComponentModel.DataAnnotations;

namespace ModuloMonolitico.Web.Pages.Gestao.TiposVendedor;

public class EditarModel : AppBasePageModel
{
    protected override string? ChavePermissao => "CRM.TiposVendedor.Visualizar";

    protected override bool ExigeEmpresaAtiva => false;

    private readonly ITipoVendedorRepositorio _tipoVendedorRepositorio;
    private readonly IMediator _mediator;

    public EditarModel(ITipoVendedorRepositorio tipoVendedorRepositorio, IMediator mediator)
    {
        _tipoVendedorRepositorio = tipoVendedorRepositorio;
        _mediator = mediator;
    }

    [BindProperty]
    public FormularioTipoVendedor Formulario { get; set; } = new();

    public class FormularioTipoVendedor
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = MensagensSistema.Gestao.TiposVendedor.NomeObrigatorio)]
        [StringLength(200, ErrorMessage = MensagensSistema.Gestao.TiposVendedor.NomeTamanhoExcedido)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = MensagensSistema.Gestao.TiposVendedor.DescricaoTamanhoExcedido)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;
    }

    [TempData]
    public string? MensagemErro { get; set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var tipo = await _mediator.Send(new ObterTipoVendedorQuery(id, ContaId));

        if (tipo == null)
        {
            Logger.LogWarning(
                "Tipo de vendedor {TipoId} não encontrado para edição pelo usuário '{Email}'.",
                id, UsuarioEmail);
            MensagemErro = MensagensSistema.Gestao.TiposVendedor.NaoEncontrado;
            return ModuloRotas.Redirecionar(ModuloRotas.Gestao.TiposVendedor);
        }

        Formulario = new FormularioTipoVendedor
        {
            Id = tipo.Id,
            Nome = tipo.Nome,
            Descricao = tipo.Descricao,
            Ativo = tipo.Ativo
        };

        Logger.LogInformation(
            "Formulário de edição do tipo de vendedor '{Nome}' (ID: {Id}) aberto pelo usuário '{Email}'.",
            tipo.Nome, tipo.Id, UsuarioEmail);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var entidade = await _tipoVendedorRepositorio.ObterPorIdEContaAsync(Formulario.Id, ContaId);

        if (entidade == null)
        {
            MensagemErro = MensagensSistema.Gestao.TiposVendedor.NaoEncontrado;
            return Page();
        }

        entidade.Nome = Formulario.Nome;
        entidade.Descricao = Formulario.Descricao;
        entidade.Ativo = Formulario.Ativo;
        entidade.DataUltimaAtualizacao = DateTime.UtcNow;

        await _tipoVendedorRepositorio.AtualizarAsync(entidade);

        MensagemSucesso = MensagensSistema.Geral.Sucesso;
        Logger.LogInformation(
            "Tipo de vendedor '{Nome}' (ID: {Id}) atualizado pelo usuário '{Email}'.",
            entidade.Nome, entidade.Id, UsuarioEmail);

        return ModuloRotas.Redirecionar(ModuloRotas.Gestao.TiposVendedor);
    }
}
