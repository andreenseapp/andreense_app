using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Servicos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Imoveis;

[AllowAnonymous]
public class BuscasRecentesModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IServicoIdentidadeJwt _identidade;

    public BuscasRecentesModel(IMediator mediator, IServicoIdentidadeJwt identidade)
    {
        _mediator = mediator;
        _identidade = identidade;
    }

    public IReadOnlyList<BuscaRecenteDto> Buscas { get; set; } = [];
    public bool UsuarioLogado { get; set; }
    public string ChaveStorageBuscasRecentes { get; set; } = ChavesSistema.StorageLocalBuscasRecentesImoveis();
    public string UrlImoveis { get; set; } = ModuloRotas.Imoveis.Index;

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        UsuarioLogado = _identidade.TentarResolverUsuarioId(User, out var usuarioId);
        if (UsuarioLogado)
            Buscas = await _mediator.Send(new ListarBuscasRecentesQuery(usuarioId));

        return Page();
    }

    public async Task<IActionResult> OnPostExcluirAsync([FromForm] Guid id)
    {
        if (!_identidade.TentarResolverUsuarioId(User, out var usuarioId))
            return Redirect(ModuloRotas.Autenticacao.Login);

        var resultado = await _mediator.Send(new DeletarFisicoBuscaRecenteCommand(id, usuarioId));
        if (resultado.Sucesso)
            MensagemSucesso = resultado.Mensagem;
        else
            MensagemErro = resultado.Mensagem;

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostLimparTodasAsync()
    {
        if (!_identidade.TentarResolverUsuarioId(User, out var usuarioId))
            return Redirect(ModuloRotas.Autenticacao.Login);

        var resultado = await _mediator.Send(new DeletarFisicoTodasBuscasRecentesCommand(usuarioId));
        if (resultado.Sucesso)
            MensagemSucesso = resultado.Mensagem;
        else
            MensagemErro = resultado.Mensagem;

        return RedirectToPage();
    }
}
