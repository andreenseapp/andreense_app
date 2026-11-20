using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Web.Options;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Api;

[ApiController]
[Route("api/agendamentos")]
public class AgendamentosApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAgendamentoQueries _repo;

    public AgendamentosApiController(IMediator mediator, IAgendamentoQueries repo)
    {
        _mediator = mediator;
        _repo     = repo;
    }

    [HttpGet("horarios")]
    public async Task<IActionResult> ObterHorarios(
        [FromQuery] Guid produtoVariacaoId,
        [FromQuery] string data,
        [FromQuery] string? prestadorIdComposto)
    {
        var empresaIdStr = Request.Cookies[EsquemaCookieAutenticacao.EmpresaAtivaId];
        if (string.IsNullOrEmpty(empresaIdStr) || !Guid.TryParse(empresaIdStr, out var empresaId))
            return Unauthorized();

        if (!DateTime.TryParse(data, out var dataParsed))
            return BadRequest("Data inválida.");

        Guid? profUsuarioId = null;
        Guid? profContatoId = null;
        if (!string.IsNullOrEmpty(prestadorIdComposto))
        {
            var parts = prestadorIdComposto.Split('|');
            if (parts.Length == 2)
            {
                if (parts[0] == "U") profUsuarioId = Guid.Parse(parts[1]);
                if (parts[0] == "C") profContatoId = Guid.Parse(parts[1]);
            }
        }

        var slots = await _repo.ObterHorariosDisponiveisAsync(empresaId, produtoVariacaoId, dataParsed, profUsuarioId, profContatoId);
        return Ok(slots.Select(s => new
        {
            s.DataHoraInicio,
            s.DataHoraFim,
            ProfissionalId = s.ProfissionalUsuarioId.HasValue ? $"U|{s.ProfissionalUsuarioId}" : $"C|{s.ProfissionalContatoId}",
            s.NomeProfissional,
            s.HoraFormatada
        }));
    }

    [HttpGet("horarios/publico/{empresaId:guid}")]
    public async Task<IActionResult> ObterHorariosPublico(
        Guid empresaId,
        [FromQuery] Guid produtoVariacaoId,
        [FromQuery] string data,
        [FromQuery] string? prestadorIdComposto)
    {
        if (!DateTime.TryParse(data, out var dataParsed))
            return BadRequest("Data inválida.");

        Guid? profUsuarioId = null;
        Guid? profContatoId = null;
        if (!string.IsNullOrEmpty(prestadorIdComposto))
        {
            var parts = prestadorIdComposto.Split('|');
            if (parts.Length == 2)
            {
                if (parts[0] == "U") profUsuarioId = Guid.Parse(parts[1]);
                if (parts[0] == "C") profContatoId = Guid.Parse(parts[1]);
            }
        }

        var slots = await _repo.ObterHorariosDisponiveisAsync(empresaId, produtoVariacaoId, dataParsed, profUsuarioId, profContatoId);
        return Ok(slots.Select(s => new
        {
            s.DataHoraInicio,
            s.DataHoraFim,
            ProfissionalId = s.ProfissionalUsuarioId.HasValue ? $"U|{s.ProfissionalUsuarioId}" : $"C|{s.ProfissionalContatoId}",
            s.NomeProfissional,
            s.HoraFormatada
        }));
    }
}



