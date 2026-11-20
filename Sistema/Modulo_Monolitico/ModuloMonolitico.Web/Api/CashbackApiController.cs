using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Api
{
    [Route("api/v1/cashback")]
    [ApiController]
    [Authorize]
    public class CashbackApiController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICashbackRepositorio _cashbackRepositorio;

        public CashbackApiController(IMediator mediator, ICashbackRepositorio cashbackRepositorio)
        {
            _mediator = mediator;
            _cashbackRepositorio = cashbackRepositorio;
        }

        [HttpGet("saldos")]
        public async Task<IActionResult> ListarMeusSaldos()
        {
            var usuarioId = ObterUsuarioId();
            var saldos = await _cashbackRepositorio.ListarSaldosPorUsuarioAsync(usuarioId);
            return Ok(saldos);
        }

        [HttpGet("historico")]
        public async Task<IActionResult> ListarMeuHistorico()
        {
            var usuarioId = ObterUsuarioId();
            var historico = await _cashbackRepositorio.ListarHistoricoCreditosPorUsuarioAsync(usuarioId);
            return Ok(historico);
        }

        [HttpPost("token/gerar")]
        public async Task<IActionResult> GerarCodigo([FromBody] GerarCodigoCashbackCommand comando)
        {
            var resultado = await _mediator.Send(comando);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }

        [HttpPost("token/consumir")]
        public async Task<IActionResult> ConsumirCodigo([FromBody] ConsumirCodigoCashbackCommand comando)
        {
            comando.UsuarioId = ObterUsuarioId();
            var resultado = await _mediator.Send(comando);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }

        [HttpPost("creditar")]
        public async Task<IActionResult> Creditar([FromBody] CreditarCashbackCommand comando)
        {
            var resultado = await _mediator.Send(comando);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }

        [HttpPost("debitar")]
        public async Task<IActionResult> Debitar([FromBody] DebitarCashbackCommand comando)
        {
            var resultado = await _mediator.Send(comando);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }

        private Guid ObterUsuarioId()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(id ?? Guid.Empty.ToString());
        }
    }
}
