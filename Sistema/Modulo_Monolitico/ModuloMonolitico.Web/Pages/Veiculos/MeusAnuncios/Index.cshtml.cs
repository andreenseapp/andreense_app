using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Veiculos.DTOs;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Pages.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Veiculos.MeusAnuncios;

public class IndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IEnumerable<AnuncioVeiculoResumoDto> MeusAnuncios { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        MeusAnuncios = await _mediator.Send(new ListarMeusAnunciosVeiculosQuery(UsuarioId));
        return Page();
    }
}
