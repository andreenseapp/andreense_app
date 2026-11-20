using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Web.Pages.Shared;
using System.ComponentModel.DataAnnotations;

namespace ModuloMonolitico.Web.Pages.Gestao.TiposVendedor;

public class NovoModel : AppBasePageModel
{
    protected override string? ChavePermissao => "CRM.TiposVendedor.Visualizar";

    protected override bool ExigeEmpresaAtiva => false;

    private readonly ITipoVendedorRepositorio _tipoVendedorRepositorio;

    public NovoModel(ITipoVendedorRepositorio tipoVendedorRepositorio) => _tipoVendedorRepositorio = tipoVendedorRepositorio;

    [BindProperty]
    public FormularioTipoVendedor Formulario { get; set; } = new();

    public class FormularioTipoVendedor
    {
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

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var entidade = new TipoVendedor
        {
            Id = Guid.NewGuid(),
            ContaId = ContaId,
            Nome = Formulario.Nome,
            Descricao = Formulario.Descricao,
            Ativo = Formulario.Ativo,
            OperadorInclusao = UsuarioId,
            DataInclusao = DateTime.UtcNow,
            DataUltimaAtualizacao = DateTime.UtcNow
        };

        await _tipoVendedorRepositorio.CadastrarAsync(entidade);

        MensagemSucesso = MensagensSistema.Geral.Sucesso;
        Logger.LogInformation(
            "Tipo de vendedor '{Nome}' cadastrado pelo usuário '{Email}'.",
            entidade.Nome, UsuarioEmail);

        return ModuloRotas.Redirecionar(ModuloRotas.Gestao.TiposVendedor);
    }
}
