using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Precos.ViewModels;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Precos;

public class EditarModel : AppBasePageModel
{
    protected override string? ChavePermissao => "ERP.ContextosPreco.Editar";

    private readonly IMediator _mediator;

    public EditarModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public FormularioContextoPreco Formulario { get; set; } = new();

    public List<Empresa> Empresas { get; set; } = [];

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? id, Guid? empresaId)
    {
        Empresas = (await _mediator.Send(new ListarEmpresasContaQuery(ContaId))).ToList();

        if (id.HasValue)
        {
            var ctx = await _mediator.Send(new ObterContextoPrecoQuery(id.Value, ContaId));
            if (ctx == null)
            {
                Logger.LogWarning(
                    "Contexto de preço {ContextoId} não encontrado para edição pelo usuário '{Email}'.",
                    id.Value, UsuarioEmail);
                MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Precos.NaoEncontrado;
                return ModuloRotas.Redirecionar(ModuloRotas.Precos.Index);
            }

            Formulario = new FormularioContextoPreco
            {
                Id = ctx.Id,
                EmpresaId = ctx.EmpresaId,
                Nome = ctx.Nome,
                Descricao = ctx.Descricao,
                EhPadrao = ctx.EhPadrao,
                Ativo = ctx.Ativo,
                Ordem = ctx.Ordem
            };

            Logger.LogInformation(
                "Formulário de edição do contexto de preço '{Nome}' (ID: {Id}) aberto pelo usuário '{Email}'.",
                ctx.Nome, ctx.Id, UsuarioEmail);
        }
        else
        {
            Formulario = new FormularioContextoPreco
            {
                EmpresaId = empresaId ?? (Empresas.FirstOrDefault()?.Id ?? Guid.Empty)
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Precos.VerifiqueDados;
            Empresas = (await _mediator.Send(new ListarEmpresasContaQuery(ContaId))).ToList();
            return Page();
        }

        try
        {
            if (Formulario.Id.HasValue)
            {
                await _mediator.Send(new AtualizarContextoPrecoCommand
                {
                    Id = Formulario.Id.Value,
                    ContaId = ContaId,
                    Nome = Formulario.Nome,
                    Descricao = Formulario.Descricao,
                    EhPadrao = Formulario.EhPadrao,
                    Ativo = Formulario.Ativo,
                    Ordem = Formulario.Ordem
                });

                Logger.LogInformation(
                    "Contexto de preço '{Nome}' (ID: {Id}) atualizado pelo usuário '{Email}'.",
                    Formulario.Nome, Formulario.Id.Value, UsuarioEmail);
            }
            else
            {
                await _mediator.Send(new CadastrarContextoPrecoCommand
                {
                    ContaId = ContaId,
                    EmpresaId = Formulario.EmpresaId,
                    Nome = Formulario.Nome,
                    Descricao = Formulario.Descricao,
                    EhPadrao = Formulario.EhPadrao,
                    Ordem = Formulario.Ordem
                });

                Logger.LogInformation(
                    "Contexto de preço '{Nome}' cadastrado pelo usuário '{Email}'.",
                    Formulario.Nome, UsuarioEmail);
            }

            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            return ModuloRotas.Redirecionar(ModuloRotas.Precos.Index, new { empresaId = Formulario.EmpresaId });
        }
        catch (Exception ex)
        {
            Logger.LogError(
                ex,
                "Erro ao salvar contexto de preço '{Nome}' pelo usuário '{Email}'.",
                Formulario.Nome, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Precos.ErroSalvar;
            Empresas = (await _mediator.Send(new ListarEmpresasContaQuery(ContaId))).ToList();
            return Page();
        }
    }
}
