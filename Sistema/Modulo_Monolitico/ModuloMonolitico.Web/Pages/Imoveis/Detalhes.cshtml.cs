using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Consultas;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Favoritos.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Servicos;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Imoveis;

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

    public AnuncioDetalheDto? Anuncio { get; set; }
    public bool UsuarioLogado { get; set; }
    public bool EhFavorito { get; set; }
    public bool EhVisualizado { get; set; }
    public string ChaveStorageVisualizados { get; set; } = ChavesSistema.StorageLocalAnunciosVisualizadosImoveis();
    public string UrlRegistrarVisualizacao { get; set; } = ModuloRotas.Imoveis.RegistrarVisualizacao;

    public async Task<IActionResult> OnGetAsync([FromQuery] Guid id)
    {
        if (id == Guid.Empty)
            return Redirect(ModuloRotas.Imoveis.Index);

        Anuncio = await _mediator.Send(new ObterAnuncioDetalheQuery(id));

        UsuarioLogado = _identidade.TentarResolverUsuarioId(User, out var usuarioId);
        if (UsuarioLogado && Anuncio != null)
        {
            EhFavorito = await _mediator.Send(new VerificarImovelFavoritoQuery(usuarioId, Anuncio.ImovelId));
            EhVisualizado = await _mediator.Send(new VerificarAnuncioVisualizadoQuery(usuarioId, id));
            await _mediator.Send(new RegistrarVisualizacaoAnuncioCommand(usuarioId, id));
        }

        return Page();
    }
}
