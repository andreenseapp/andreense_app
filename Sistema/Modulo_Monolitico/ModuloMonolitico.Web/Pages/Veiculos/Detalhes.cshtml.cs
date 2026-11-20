using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using ModuloMonolitico.Dominio.Veiculos.DTOs;
using ModuloMonolitico.Web.Servicos;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Veiculos;

[AllowAnonymous]
public class DetalhesModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IServicoIdentidadeJwt _identidade;

    public DetalhesModel(IMediator mediator, IServicoIdentidadeJwt identidade)
    {
        _mediator = mediator;
        _identidade = identidade;
    }

    public AnuncioVeiculoDetalheDto? Anuncio { get; set; }
    public bool UsuarioLogado { get; set; }
    public bool UsuarioEhMaster { get; set; }
    public bool EhProprietario { get; set; }

    public async Task<IActionResult> OnGetAsync([FromQuery] Guid id)
    {
        if (id == Guid.Empty)
            return RedirectToPage("/Veiculos/Index");

        Anuncio = await _mediator.Send(new ObterAnuncioVeiculoDetalheQuery(id));

        if (Anuncio is null)
            return NotFound();

        UsuarioLogado = _identidade.TentarResolverUsuarioId(User, out var usuarioId);
        UsuarioEhMaster = UsuarioLogado && _identidade.EhUsuarioMaster(usuarioId, _identidade.ObterEmail(User));
        EhProprietario = UsuarioLogado && Anuncio.UsuarioId == usuarioId;

        return Page();
    }
}
