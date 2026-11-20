using ModuloMonolitico.Web.Core.Constantes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Web.Pages.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Imobiliaria.MeusAnuncios;

public class IndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public IEnumerable<AnuncioResumoDto> Anuncios { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        Anuncios = await _mediator.Send(new ListarMeusAnunciosQuery(UsuarioId));
        return Page();
    }

    public async Task<IActionResult> OnPostAtualizarSituacaoAsync([FromForm] Guid anuncioId, [FromForm] string novaSituacao)
    {
        var resultado = await _mediator.Send(new AtualizarSituacaoAnuncioCommand(anuncioId, UsuarioId, novaSituacao));

        if (resultado.Sucesso)
            NotificarSucesso(MensagensSistema.Geral.Sucesso);
        else
            NotificarErro(resultado.Mensagem);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAlternarDestaqueAsync([FromForm] Guid anuncioId)
    {
        var resultado = await _mediator.Send(new AlternarDestaqueAnuncioCommand(anuncioId, UsuarioId));

        if (resultado.Sucesso)
            NotificarSucesso(resultado.Dados ? MensagensSistema.Imobiliaria.DestaqueAtivado : MensagensSistema.Imobiliaria.DestaqueDesativado);
        else
            NotificarErro(resultado.Mensagem);

        return RedirectToPage();
    }
}
