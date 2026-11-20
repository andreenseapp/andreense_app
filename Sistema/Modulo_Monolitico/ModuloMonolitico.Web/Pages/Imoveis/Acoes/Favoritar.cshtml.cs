using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Favoritos.Comandos;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Servicos;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Imoveis.Acoes;

[AllowAnonymous]
[IgnoreAntiforgeryToken]
public class FavoritarModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IServicoIdentidadeJwt _identidade;
    private readonly ILogger<FavoritarModel> _logger;

    public FavoritarModel(
        IMediator mediator,
        IServicoIdentidadeJwt identidade,
        ILogger<FavoritarModel> logger)
    {
        _mediator = mediator;
        _identidade = identidade;
        _logger = logger;
    }

    public async Task<IActionResult> OnPostAsync([FromBody] FavoritarRequest request)
    {
        if (!_identidade.TentarResolverUsuarioId(User, out var usuarioId))
            return Unauthorized();

        if (request == null || request.ImovelId == Guid.Empty)
            return BadRequest();

        try
        {
            var favoritado = await _mediator.Send(new AlternarImovelFavoritoCommand(usuarioId, request.ImovelId));
            return new JsonResult(new { sucesso = true, favoritado });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao alternar favorito do imóvel {ImovelId} para usuário {UsuarioId}", request.ImovelId, usuarioId);
            return ModuloRotas.JsonErro(MensagensSistema.Imobiliaria.ErroFavoritarImovel);
        }
    }
}

public class FavoritarRequest
{
    public Guid ImovelId { get; set; }
}
