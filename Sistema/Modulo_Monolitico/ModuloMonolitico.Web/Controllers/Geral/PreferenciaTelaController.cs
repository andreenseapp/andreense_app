using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Geral.PreferenciaTela;
using System.Security.Claims;

namespace ModuloMonolitico.Web.Controllers.Geral;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PreferenciaTelaController : ControllerBase
{
    private readonly IMediator _mediator;

    public PreferenciaTelaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("salvar")]
    public async Task<IActionResult> AtualizarPreferencia([FromBody] RequisicaoAtualizarPreferenciaTela request)
    {
        var contaIdStr = User.FindFirstValue("conta_id");
        var usuarioIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(contaIdStr) || !Guid.TryParse(contaIdStr, out var contaId) ||
            string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out var usuarioId))
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.TelaIdentificador) || string.IsNullOrWhiteSpace(request.ColunasVisiveisJson))
        {
            return BadRequest("Dados inválidos.");
        }

        var command = new AtualizarPreferenciaTelaCommand(contaId, usuarioId, request.TelaIdentificador, request.ColunasVisiveisJson);
        await _mediator.Send(command);

        return Ok(new { sucesso = true });
    }
}
