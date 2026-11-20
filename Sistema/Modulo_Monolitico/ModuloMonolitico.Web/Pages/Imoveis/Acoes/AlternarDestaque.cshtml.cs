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

[Authorize]
[IgnoreAntiforgeryToken]
public class AlternarDestaqueModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IServicoIdentidadeJwt _identidade;
    private readonly ILogger<AlternarDestaqueModel> _logger;

    public AlternarDestaqueModel(
        IMediator mediator,
        IServicoIdentidadeJwt identidade,
        ILogger<AlternarDestaqueModel> logger)
    {
        _mediator = mediator;
        _identidade = identidade;
        _logger = logger;
    }

    public async Task<IActionResult> OnPostAsync([FromBody] AlternarDestaqueRequest? request)
    {
        if (!_identidade.TentarResolverUsuarioId(User, out var usuarioId))
            return Unauthorized();

        var email = _identidade.ObterEmail(User);
        if (!_identidade.EhUsuarioMaster(usuarioId, email))
        {
            _logger.LogWarning(
                "Usuário {UsuarioId} tentou alterar destaque sem ser administrador master",
                usuarioId);
            return Forbid();
        }

        if (request is null || request.AnuncioId == Guid.Empty)
            return BadRequest();

        try
        {
            var resultado = await _mediator.Send(
                new AlternarDestaqueAnuncioCommand(request.AnuncioId, usuarioId, EhAdministradorMaster: true));

            if (!resultado.Sucesso)
                return ModuloRotas.JsonErro(resultado.Mensagem);

            return new JsonResult(new
            {
                sucesso = true,
                ehDestaque = resultado.Dados,
                mensagem = resultado.Mensagem
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao alternar destaque do anúncio {AnuncioId}", request.AnuncioId);
            return ModuloRotas.JsonErro(MensagensSistema.Imobiliaria.ErroAlternarDestaque);
        }
    }
}

public class AlternarDestaqueRequest
{
    public Guid AnuncioId { get; set; }
}
