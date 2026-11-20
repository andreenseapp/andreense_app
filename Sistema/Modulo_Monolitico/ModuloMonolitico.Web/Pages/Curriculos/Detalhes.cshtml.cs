using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;
using ModuloMonolitico.Dominio.Curriculos.DTOs;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Curriculos;

[AllowAnonymous]
public class DetalhesModel : PageModel
{
    private readonly IMediator _mediator;

    public DetalhesModel(IMediator mediator) => _mediator = mediator;

    public CurriculoDetalheDto? Curriculo { get; set; }

    public async Task<IActionResult> OnGetAsync([FromQuery] Guid id)
    {
        if (id == Guid.Empty)
            return NotFound();

        if (!Guid.TryParse(User.FindFirst("conta_id")?.Value, out var contaId))
            contaId = Guid.Empty;

        Curriculo = await _mediator.Send(new ObterCurriculoDetalheQuery(contaId, id));

        if (Curriculo is null)
            return NotFound();

        return Page();
    }
}
