using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediatR;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Favoritos.Consultas;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Servicos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Imoveis;

[AllowAnonymous]
public class AnuncianteModel : PageModel
{
    private readonly IPerfilAnuncianteRepositorio _perfilRepositorio;
    private readonly IMediator _mediator;
    private readonly IServicoIdentidadeJwt _identidade;

    public AnuncianteModel(
        IPerfilAnuncianteRepositorio perfilRepositorio,
        IMediator mediator,
        IServicoIdentidadeJwt identidade)
    {
        _perfilRepositorio = perfilRepositorio;
        _mediator = mediator;
        _identidade = identidade;
    }

    public PerfilAnuncianteDto Perfil { get; set; } = default!;
    public ResultadoPaginado<AnuncioResumoDto> Anuncios { get; set; } = default!;
    public List<Guid> ImoveisFavoritosIds { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public FiltroAnuncio Filtro { get; set; } = new();

    public bool FiltrosAtivos =>
        !string.IsNullOrEmpty(Filtro.TipoNegocio) ||
        !string.IsNullOrEmpty(Filtro.TipoImovel) ||
        !string.IsNullOrEmpty(Filtro.Bairro) ||
        Filtro.QuartosMinimo.HasValue ||
        Filtro.BanheirosMinimo.HasValue ||
        Filtro.VagasMinimo.HasValue ||
        Filtro.ValorMinimo.HasValue ||
        Filtro.ValorMaximo.HasValue ||
        Filtro.ValorCondominioMinimo.HasValue ||
        Filtro.ValorCondominioMaximo.HasValue ||
        Filtro.AreaMinima.HasValue ||
        Filtro.AreaMaxima.HasValue ||
        !string.IsNullOrWhiteSpace(Filtro.SituacaoObra) ||
        !string.IsNullOrWhiteSpace(Filtro.CodigoPublico) ||
        (Filtro.CodigosAmenidade?.Count > 0) ||
        Filtro.BuscarPorPacote;

    public string QueryStringFiltros
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(Filtro.TipoNegocio)) parts.Add($"&Filtro.TipoNegocio={Uri.EscapeDataString(Filtro.TipoNegocio)}");
            if (!string.IsNullOrWhiteSpace(Filtro.TipoImovel)) parts.Add($"&Filtro.TipoImovel={Uri.EscapeDataString(Filtro.TipoImovel)}");
            if (!string.IsNullOrWhiteSpace(Filtro.Bairro)) parts.Add($"&Filtro.Bairro={Uri.EscapeDataString(Filtro.Bairro)}");
            if (Filtro.QuartosMinimo.HasValue) parts.Add($"&Filtro.QuartosMinimo={Filtro.QuartosMinimo}");
            if (Filtro.BanheirosMinimo.HasValue) parts.Add($"&Filtro.BanheirosMinimo={Filtro.BanheirosMinimo}");
            if (Filtro.VagasMinimo.HasValue) parts.Add($"&Filtro.VagasMinimo={Filtro.VagasMinimo}");
            if (Filtro.ValorMinimo.HasValue) parts.Add($"&Filtro.ValorMinimo={Filtro.ValorMinimo}");
            if (Filtro.ValorMaximo.HasValue) parts.Add($"&Filtro.ValorMaximo={Filtro.ValorMaximo}");
            if (Filtro.ValorCondominioMinimo.HasValue) parts.Add($"&Filtro.ValorCondominioMinimo={Filtro.ValorCondominioMinimo}");
            if (Filtro.ValorCondominioMaximo.HasValue) parts.Add($"&Filtro.ValorCondominioMaximo={Filtro.ValorCondominioMaximo}");
            if (Filtro.AreaMinima.HasValue) parts.Add($"&Filtro.AreaMinima={Filtro.AreaMinima}");
            if (Filtro.AreaMaxima.HasValue) parts.Add($"&Filtro.AreaMaxima={Filtro.AreaMaxima}");
            if (!string.IsNullOrWhiteSpace(Filtro.SituacaoObra)) parts.Add($"&Filtro.SituacaoObra={Uri.EscapeDataString(Filtro.SituacaoObra)}");
            if (!string.IsNullOrWhiteSpace(Filtro.CodigoPublico)) parts.Add($"&Filtro.CodigoPublico={Uri.EscapeDataString(Filtro.CodigoPublico)}");
            if (Filtro.BuscarPorPacote) parts.Add("&Filtro.BuscarPorPacote=true");
            if (Filtro.CodigosAmenidade is { Count: > 0 })
            {
                foreach (var codigo in Filtro.CodigosAmenidade.Where(c => !string.IsNullOrWhiteSpace(c)))
                    parts.Add($"&Filtro.CodigosAmenidade={Uri.EscapeDataString(codigo)}");
            }
            return string.Concat(parts);
        }
    }

    public async Task<IActionResult> OnGetAsync(Guid? id, Guid? contaId)
    {
        id ??= contaId;

        if (id == null || id == Guid.Empty)
            return Redirect(ModuloRotas.Imoveis.Index);

        var perfil = await _perfilRepositorio.ObterPorContaIdAsync(id.Value);

        if (perfil == null)
            return Redirect(ModuloRotas.Imoveis.Index);

        Perfil = perfil;

        if (Filtro.Pagina < 1)
            Filtro.Pagina = 1;

        var anunciosTask = _perfilRepositorio.ListarAnunciosPorContaAsync(id.Value, Filtro);
        Task<List<Guid>> favoritosTask = Task.FromResult(new List<Guid>());

        if (_identidade.TentarResolverUsuarioId(User, out var usuarioId))
            favoritosTask = _mediator.Send(new ObterImoveisFavoritosQuery(usuarioId));

        await Task.WhenAll(anunciosTask, favoritosTask);

        Anuncios = await anunciosTask;
        ImoveisFavoritosIds = await favoritosTask;

        return Page();
    }
}
