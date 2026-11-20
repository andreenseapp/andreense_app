using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Estoque.ViewModels;
using ModuloMonolitico.Web.Pages.Shared;
using ProdutoEntidade = ModuloMonolitico.Dominio.Entidades.Produto;

namespace ModuloMonolitico.Web.Pages.Estoque;

public class EntradaModel : AppBasePageModel
{
    protected override string? ChavePermissao => "ERP.Estoque.Entrada";

    private readonly IMediator _mediator;

    public EntradaModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public FormularioEntrada Formulario { get; set; } = new();

    public List<Empresa> Empresas { get; set; } = [];
    public List<ProdutoEntidade> Produtos { get; set; } = [];
    public List<LocalEstoque> Locais { get; set; } = [];

    [TempData]
    public string? MensagemErro { get; set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? empresaId, Guid? variacaoId)
    {
        Empresas = (await _mediator.Send(new ListarEmpresasContaQuery(ContaId))).ToList();
        var empId = empresaId ?? (Empresas.FirstOrDefault()?.Id ?? Guid.Empty);

        Formulario.EmpresaId = empId;
        if (variacaoId.HasValue)
            Formulario.ProdutoVariacaoId = variacaoId.Value;

        if (empId != Guid.Empty)
            await RecarregarDependenciasAsync(ContaId, empId);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || Formulario.Quantidade <= 0)
        {
            Logger.LogWarning(
                "Validação falhou ao registrar entrada de estoque na Empresa {EmpresaId} pelo usuário '{Email}'.",
                Formulario.EmpresaId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Estoque.QuantidadeEntradaInvalida;
            await RecarregarDependenciasAsync(ContaId, Formulario.EmpresaId);
            return Page();
        }

        try
        {
            var cmd = new CadastrarSaldoEstoqueCommand
            {
                ContaId = ContaId,
                EmpresaId = Formulario.EmpresaId,
                ProdutoVariacaoId = Formulario.ProdutoVariacaoId,
                LocalEstoqueId = Formulario.LocalEstoqueId,
                Lote = Formulario.Lote,
                DataValidade = Formulario.DataValidade,
                DataFabricacao = Formulario.DataFabricacao,
                Quantidade = Formulario.Quantidade,
                Motivo = Formulario.Motivo,
                UsuarioResponsavelId = UsuarioId == Guid.Empty ? null : UsuarioId
            };

            await _mediator.Send(cmd);

            Logger.LogInformation(
                "Entrada de {Quantidade} unidade(s) da variação {VariacaoId} registrada na Empresa {EmpresaId} pelo usuário '{Email}'.",
                Formulario.Quantidade, Formulario.ProdutoVariacaoId, Formulario.EmpresaId, UsuarioEmail);
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            return ModuloRotas.Redirecionar(ModuloRotas.Estoque.Index, new { empresaId = Formulario.EmpresaId });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex,
                "Erro ao registrar entrada de estoque na Empresa {EmpresaId} pelo usuário '{Email}'.",
                Formulario.EmpresaId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Estoque.ErroRegistrarEntrada;
            await RecarregarDependenciasAsync(ContaId, Formulario.EmpresaId);
            return Page();
        }
    }

    private async Task RecarregarDependenciasAsync(Guid contaId, Guid empresaId)
    {
        Empresas = (await _mediator.Send(new ListarEmpresasContaQuery(contaId))).ToList();
        Produtos = (await _mediator.Send(new ListarProdutosAtivosQuery(contaId))).ToList();
        Locais = (await _mediator.Send(new ListarLocaisEstoqueQuery(contaId, empresaId))).ToList();
    }
}
