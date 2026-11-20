using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Web.Pages.Produto.ViewModels;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Produto;

public class EditarModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Produto.Editar";

    private readonly IMediator _mediator;

    public EditarModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public List<string> CategoriasDisponiveis { get; set; } = [];
    public List<string> MarcasDisponiveis { get; set; } = [];
    public List<UnidadeMedidaViewModel> UnidadesDisponiveis { get; set; } = [];

    [BindProperty]
    public ProdutoFormViewModel Comando { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        if (id == Guid.Empty)
            return ModuloRotas.Redirecionar(ModuloRotas.Produto.Index);

        var produto = await _mediator.Send(new ObterProdutoQuery(id, ContaId));
        if (produto == null)
            return ModuloRotas.Redirecionar(ModuloRotas.Produto.Index);

        Comando = new ProdutoFormViewModel
        {
            Id = produto.Id,
            Codigo = produto.Codigo,
            Descricao = produto.Descricao,
            DescricaoComplementar = produto.DescricaoComplementar,
            TipoProduto = produto.TipoProduto,
            Grupo = produto.Grupo,
            Marca = produto.Marca,
            UnidadeMedida = produto.UnidadeMedida,
            Ncm = produto.Ncm,
            Cest = produto.Cest,
            Observacao = produto.Observacao,
            Ativo = produto.Ativo,
            Variacoes = produto.Variacoes.Select(v => new ProdutoVariacaoFormViewModel
            {
                Id = v.Id,
                Descricao = v.Descricao,
                CodigoBarras = v.CodigoBarras,
                CodigoSku = v.CodigoSku,
                PrecoVenda = v.PrecoVenda,
                Custo = v.Custo,
                EstoqueAtual = v.EstoqueAtual,
                EstoqueMinimo = v.EstoqueMinimo
            }).ToList()
        };

        await CarregarListasAsync(ContaId);
        return Page();
    }

    private async Task CarregarListasAsync(Guid contaId)
    {
        CategoriasDisponiveis = (await _mediator.Send(new ListarCategoriasProdutoQuery(contaId))).ToList();
        MarcasDisponiveis = (await _mediator.Send(new ListarMarcasProdutoQuery(contaId))).ToList();

        var unidades = await _mediator.Send(new ListarUnidadesMedidaQuery(contaId));
        UnidadesDisponiveis = unidades.Select(u => new UnidadeMedidaViewModel { Sigla = u.Sigla, Nome = u.Nome }).ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CarregarListasAsync(ContaId);
            MensagemErro = MensagensSistema.Produto.VerifiqueDados;
            return Page();
        }

        if (!Comando.Id.HasValue)
        {
            Logger.LogWarning(
                "Falha ao atualizar Produto na Conta {ContaId}: ID não fornecido pelo usuário '{Email}'.",
                ContaId, UsuarioEmail);
            MensagemErro = MensagensSistema.Produto.IdNaoFornecido;
            return Page();
        }

        try
        {
            if (Comando.Variacoes.Count == 1 && string.IsNullOrWhiteSpace(Comando.Variacoes[0].CodigoSku))
                Comando.Variacoes[0].CodigoSku = Comando.Codigo;

            if (Comando.Variacoes.Count > 1 && Comando.Variacoes.Any(v => string.IsNullOrWhiteSpace(v.CodigoSku)))
            {
                await CarregarListasAsync(ContaId);
                MensagemErro = MensagensSistema.Produto.VariacoesPrecisamSKU;
                return Page();
            }

            var variacoesDto = Comando.Variacoes.Select(v => new VariacaoDto
            {
                Id = v.Id,
                Descricao = v.Descricao,
                CodigoBarras = v.CodigoBarras,
                CodigoSku = v.CodigoSku,
                PrecoVenda = v.PrecoVenda,
                Custo = v.Custo,
                EstoqueAtual = v.EstoqueAtual,
                EstoqueMinimo = v.EstoqueMinimo
            }).ToList();

            await _mediator.Send(new AtualizarProdutoCommand
            {
                Id = Comando.Id.Value,
                ContaId = ContaId,
                Codigo = Comando.Codigo,
                Descricao = Comando.Descricao,
                DescricaoComplementar = Comando.DescricaoComplementar,
                TipoProduto = Comando.TipoProduto,
                Grupo = Comando.Grupo,
                Marca = Comando.Marca,
                UnidadeMedida = Comando.UnidadeMedida,
                Ncm = Comando.Ncm,
                Cest = Comando.Cest,
                Observacao = Comando.Observacao,
                Ativo = Comando.Ativo,
                Variacoes = variacoesDto
            });

            Logger.LogInformation(
                "Produto '{Descricao}' ({TipoProduto}, ID: {Id}) atualizado com sucesso na Conta {ContaId} pelo usuário '{Email}'.",
                Comando.Descricao, Comando.TipoProduto, Comando.Id, ContaId, UsuarioEmail);
            MensagemSucesso = MensagensSistema.Geral.Sucesso;

            return ModuloRotas.Redirecionar(ModuloRotas.Produto.Index);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao atualizar Produto na Conta {ContaId} pelo usuário '{Email}'.", ContaId, UsuarioEmail);
            await CarregarListasAsync(ContaId);
            MensagemErro = MensagensSistema.Produto.ErroSalvar;
            return Page();
        }
    }

    public async Task<IActionResult> OnPostCadastrarCategoriaAsync(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return ModuloRotas.JsonErro(MensagensSistema.Validacao.NomeObrigatorio);

        await _mediator.Send(new CadastrarCategoriaProdutoCommand(ContaId, nome));

        Logger.LogInformation(
            "Nova Categoria de Produto '{Nome}' cadastrada via modal na Conta {ContaId} pelo usuário '{Email}'.",
            nome.Trim(), ContaId, UsuarioEmail);
        return new JsonResult(new { sucesso = true, nome = nome.Trim() });
    }

    public async Task<IActionResult> OnPostCadastrarMarcaAsync(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return ModuloRotas.JsonErro(MensagensSistema.Validacao.NomeObrigatorio);

        await _mediator.Send(new CadastrarMarcaProdutoCommand(ContaId, nome));

        Logger.LogInformation(
            "Nova Marca '{Nome}' cadastrada via modal na Conta {ContaId} pelo usuário '{Email}'.",
            nome.Trim(), ContaId, UsuarioEmail);
        return new JsonResult(new { sucesso = true, nome = nome.Trim() });
    }

    public async Task<IActionResult> OnPostCadastrarUnidadeAsync(string sigla, string nome)
    {
        if (string.IsNullOrWhiteSpace(sigla))
            return ModuloRotas.JsonErro(MensagensSistema.Validacao.SiglaObrigatoria);

        if (string.IsNullOrWhiteSpace(nome))
            return ModuloRotas.JsonErro(MensagensSistema.Validacao.NomeObrigatorio);

        var resultado = await _mediator.Send(new CadastrarUnidadeMedidaCommand(ContaId, sigla, nome));

        if (!resultado.Sucesso)
            return ModuloRotas.JsonErro(resultado.Mensagem ?? MensagensSistema.Geral.ErroDesconhecido);

        Logger.LogInformation(
            "Nova Unidade de Medida '{Sigla}' cadastrada via modal na Conta {ContaId} pelo usuário '{Email}'.",
            sigla.Trim().ToUpperInvariant(), ContaId, UsuarioEmail);
        return new JsonResult(new { sucesso = true, sigla = sigla.Trim().ToUpperInvariant(), nome = nome.Trim() });
    }
}
