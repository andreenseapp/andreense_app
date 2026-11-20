using System.ComponentModel.DataAnnotations;
using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Empresas;

public class EditarModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Base.Empresas.Editar";
    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;

    public EditarModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty]
    public EmpresaFormulario Formulario { get; set; } = new();

    public Guid? Id { get; set; }

    [TempData] public string? MensagemSucesso { get; set; }
    [TempData] public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        Id = id;

        if (!id.HasValue) return Page();

        var empresa = await _mediator.Send(new ObterEmpresaQuery(id.Value, ContaId));
        if (empresa is null)
        {
            Logger.LogWarning(
                "Empresa {Id} não encontrada para edição pelo usuário '{Email}'. ContaId: {ContaId}",
                id, UsuarioEmail, ContaId);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Empresas.NaoEncontrada;
            return ModuloRotas.Redirecionar(ModuloRotas.Empresas.Index);
        }

        Formulario.Nome             = empresa.Nome;
        Formulario.RazaoSocial      = empresa.RazaoSocial;
        Formulario.Cnpj             = empresa.Cnpj;
        Formulario.InscricaoEstadual= empresa.InscricaoEstadual;
        Formulario.Segmento         = empresa.Segmento;
        Formulario.CnaePrincipal    = empresa.CnaePrincipal;
        Formulario.CnaesSecundarios = empresa.CnaesSecundarios != null && empresa.CnaesSecundarios.Any()
            ? string.Join(", ", empresa.CnaesSecundarios.Select(x => x.Cnae))
            : null;
        Formulario.Cep              = empresa.Cep;
        Formulario.Logradouro       = empresa.Logradouro;
        Formulario.Numero           = empresa.Numero;
        Formulario.Bairro           = empresa.Bairro;
        Formulario.Cidade           = empresa.Cidade;
        Formulario.Uf               = empresa.Uf;

        Logger.LogInformation(
            "Formulário de edição da empresa '{Nome}' (ID: {Id}) aberto pelo usuário '{Email}'.",
            empresa.Nome, id, UsuarioEmail);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid? id)
    {
        Id = id;

        if (!ModelState.IsValid) return Page();

        ResultadoDto resultado;
        if (id.HasValue)
        {
            resultado = await _mediator.Send(new AtualizarEmpresaCommand
            {
                Id = id.Value,
                ContaId = ContaId,
                Nome = Formulario.Nome,
                RazaoSocial = Formulario.RazaoSocial,
                Cnpj = Formulario.Cnpj,
                InscricaoEstadual = Formulario.InscricaoEstadual,
                Segmento = Formulario.Segmento,
                CnaePrincipal = Formulario.CnaePrincipal,
                CnaesSecundarios = Formulario.CnaesSecundarios,
                Cep = Formulario.Cep,
                Logradouro = Formulario.Logradouro,
                Numero = Formulario.Numero,
                Bairro = Formulario.Bairro,
                Cidade = Formulario.Cidade,
                Uf = Formulario.Uf
            });
        }
        else
        {
            resultado = await _mediator.Send(new CadastrarEmpresaCommand
            {
                ContaId = ContaId,
                Nome = Formulario.Nome,
                RazaoSocial = Formulario.RazaoSocial,
                Cnpj = Formulario.Cnpj,
                InscricaoEstadual = Formulario.InscricaoEstadual,
                Segmento = Formulario.Segmento,
                CnaePrincipal = Formulario.CnaePrincipal,
                CnaesSecundarios = Formulario.CnaesSecundarios,
                Cep = Formulario.Cep,
                Logradouro = Formulario.Logradouro,
                Numero = Formulario.Numero,
                Bairro = Formulario.Bairro,
                Cidade = Formulario.Cidade,
                Uf = Formulario.Uf
            });
        }

        if (!resultado.Sucesso)
        {
            Logger.LogWarning(
                "Falha ao salvar empresa {Id} pelo usuário '{Email}': {Mensagem}",
                id, UsuarioEmail, resultado.Mensagem);
            ModelState.AddModelError(string.Empty, resultado.Mensagem ?? "Não foi possível salvar a empresa.");
            return Page();
        }

        if (id.HasValue)
            Logger.LogInformation("Empresa '{Nome}' (ID: {Id}) atualizada pelo usuário '{Email}'.", Formulario.Nome, id, UsuarioEmail);
        else
            Logger.LogInformation("Nova empresa '{Nome}' criada pelo usuário '{Email}'.", Formulario.Nome, UsuarioEmail);

        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return ModuloRotas.Redirecionar(ModuloRotas.Empresas.Index);
    }

    public class EmpresaFormulario
    {
        [Display(Name = "Nome fantasia")]
        [Required(ErrorMessage = "O nome fantasia é obrigatório.")]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Razão social")]
        [MaxLength(200)]
        public string? RazaoSocial { get; set; }

        [Display(Name = "CNPJ")]
        [MaxLength(18)]
        public string? Cnpj { get; set; }

        [Display(Name = "Inscrição estadual")]
        [MaxLength(30)]
        public string? InscricaoEstadual { get; set; }

        [Display(Name = "Segmento")]
        [MaxLength(100)]
        public string? Segmento { get; set; }

        [Display(Name = "CNAE principal")]
        [MaxLength(20)]
        public string? CnaePrincipal { get; set; }

        [Display(Name = "CNAEs secundários")]
        [MaxLength(500)]
        public string? CnaesSecundarios { get; set; }

        [Display(Name = "CEP")]
        [MaxLength(9)]
        public string? Cep { get; set; }

        [Display(Name = "Logradouro")]
        [MaxLength(200)]
        public string? Logradouro { get; set; }

        [Display(Name = "Número")]
        [MaxLength(20)]
        public string? Numero { get; set; }

        [Display(Name = "Bairro")]
        [MaxLength(100)]
        public string? Bairro { get; set; }

        [Display(Name = "Cidade")]
        [MaxLength(100)]
        public string? Cidade { get; set; }

        [Display(Name = "UF")]
        [MaxLength(2)]
        public string? Uf { get; set; }
    }
}
