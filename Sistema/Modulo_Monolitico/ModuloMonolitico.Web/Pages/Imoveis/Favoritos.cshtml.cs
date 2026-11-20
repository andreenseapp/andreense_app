using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Favoritos.Consultas;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Servicos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Imoveis;

[AllowAnonymous]
public class FavoritosModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IServicoIdentidadeJwt _identidade;

    public FavoritosModel(IMediator mediator, IServicoIdentidadeJwt identidade)
    {
        _mediator = mediator;
        _identidade = identidade;
    }

    public ResultadoPaginado<AnuncioResumoDto> Resultado { get; set; } = new();
    public IReadOnlyList<Guid> FavoritosIds { get; set; } = [];
    public bool UsuarioLogado { get; set; }
    public string UrlImoveis { get; set; } = ModuloRotas.Imoveis.Index;
    public string UrlLogin { get; set; } = ModuloRotas.Autenticacao.Login;

    public async Task<IActionResult> OnGetAsync([FromQuery] int pagina = 1)
    {
        UsuarioLogado = _identidade.TentarResolverUsuarioId(User, out var usuarioId);
        if (!UsuarioLogado)
        {
            var retorno = Uri.EscapeDataString(ModuloRotas.Imoveis.Favoritos);
            return Redirect($"{ModuloRotas.Autenticacao.Login}?ReturnUrl={retorno}");
        }

        Resultado = await _mediator.Send(new ListarAnunciosFavoritosQuery(usuarioId, pagina));
        FavoritosIds = Resultado.Itens.Select(a => a.ImovelId).ToList();

        return Page();
    }
}
