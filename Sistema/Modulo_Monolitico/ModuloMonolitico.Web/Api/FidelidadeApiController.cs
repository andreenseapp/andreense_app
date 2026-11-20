using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Api
{
    [ApiController]
    [Route("api/v1/fidelidade")]
    public class FidelidadeApiController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FidelidadeApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("token/gerar")]
        public async Task<IActionResult> GerarCodigo([FromBody] GerarCodigoFidelidadeCommand comando)
        {
            var resultado = await _mediator.Send(comando);
            if (resultado.Sucesso) return Ok(resultado);
            return BadRequest(resultado);
        }

        [HttpPost("token/consumir")]
        public async Task<IActionResult> ConsumirCodigo([FromBody] ConsumirCodigoFidelidadeCommand comando)
        {
            var resultado = await _mediator.Send(comando);
            if (resultado.Sucesso) return Ok(resultado);
            return BadRequest(resultado);
        }

        [HttpPost("resgatar")]
        public async Task<IActionResult> ResgatarRecompensa([FromBody] ResgatarRecompensaCommand comando)
        {
            var resultado = await _mediator.Send(comando);
            if (resultado.Sucesso) return Ok(resultado);
            return BadRequest(resultado);
        }
    }
}



