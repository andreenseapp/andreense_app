using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Servicos;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Imoveis.Acoes;

[AllowAnonymous]
[IgnoreAntiforgeryToken]
public class RegistrarVisualizacaoModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IServicoIdentidadeJwt _identidade;
    private readonly ILogger<RegistrarVisualizacaoModel> _logger;

    public RegistrarVisualizacaoModel(
        IMediator mediator,
        IServicoIdentidadeJwt identidade,
        ILogger<RegistrarVisualizacaoModel> logger)
    {
        _mediator = mediator;
        _identidade = identidade;
        _logger = logger;
    }

    public async Task<IActionResult> OnPostAsync([FromBody] RegistrarVisualizacaoRequest request)
    {
        if (!_identidade.TentarResolverUsuarioId(User, out var usuarioId))
            return Unauthorized();

        if (request == null || request.AnuncioId == Guid.Empty)
            return BadRequest();

        try
        {
            await _mediator.Send(new RegistrarVisualizacaoAnuncioCommand(usuarioId, request.AnuncioId));
            return new JsonResult(new { sucesso = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao registrar visualização do anúncio {AnuncioId} para usuário {UsuarioId}",
                request.AnuncioId, usuarioId);
            return ModuloRotas.JsonErro(MensagensSistema.Imobiliaria.ErroRegistrarVisualizacao);
        }
    }
}

public class RegistrarVisualizacaoRequest
{
    public Guid AnuncioId { get; set; }
}
