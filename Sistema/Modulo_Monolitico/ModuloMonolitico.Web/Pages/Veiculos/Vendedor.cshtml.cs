using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using ModuloMonolitico.Dominio.Veiculos.DTOs;
using ModuloMonolitico.Web.Servicos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Veiculos;

[AllowAnonymous]
public class VendedorModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IPerfilVendedorRepositorio _perfilRepositorio;
    private readonly IAnuncioVeiculoRepositorio _anuncioRepositorio;
    private readonly IServicoIdentidadeJwt _identidade;

    public VendedorModel(
        IMediator mediator,
        IPerfilVendedorRepositorio perfilRepositorio,
        IAnuncioVeiculoRepositorio anuncioRepositorio,
        IServicoIdentidadeJwt identidade)
    {
        _mediator = mediator;
        _perfilRepositorio = perfilRepositorio;
        _anuncioRepositorio = anuncioRepositorio;
        _identidade = identidade;
    }

    public PerfilVendedorDto? Perfil { get; set; }
    public IEnumerable<AnuncioVeiculoResumoDto> AnunciosDoVendedor { get; set; } = [];
    public bool UsuarioLogado { get; set; }

    public async Task<IActionResult> OnGetAsync([FromQuery] Guid contaId, [FromQuery] int pagina = 1)
    {
        if (contaId == Guid.Empty)
            return RedirectToPage("/Veiculos/Index");

        Perfil = await _perfilRepositorio.ObterPorContaAsync(contaId);

        if (Perfil is null)
            return NotFound();

        AnunciosDoVendedor = await _anuncioRepositorio.ListarPorVendedorAsync(contaId, pagina, 12);
        UsuarioLogado = _identidade.TentarResolverUsuarioId(User, out _);

        return Page();
    }
}
