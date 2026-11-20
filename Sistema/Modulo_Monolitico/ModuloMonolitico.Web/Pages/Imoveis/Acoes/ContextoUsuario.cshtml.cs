using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Consultas;
using ModuloMonolitico.Dominio.Imobiliaria.Servicos;
using ModuloMonolitico.Web.Servicos;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Imoveis.Acoes;

[Authorize]
[IgnoreAntiforgeryToken]
public class ContextoUsuarioModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IServicoIdentidadeJwt _identidade;

    public ContextoUsuarioModel(IMediator mediator, IServicoIdentidadeJwt identidade)
    {
        _mediator = mediator;
        _identidade = identidade;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!_identidade.TentarResolverUsuarioId(User, out var usuarioId))
            return Unauthorized();

        var contexto = await _mediator.Send(new ObterContextoUsuarioImoveisQuery(usuarioId));
        return new JsonResult(new { sucesso = true, dados = contexto });
    }

    public async Task<IActionResult> OnPostRegistrarBuscaAsync([FromBody] RegistrarBuscaRequest? request)
    {
        if (!_identidade.TentarResolverUsuarioId(User, out var usuarioId))
            return Unauthorized();

        if (request?.Filtro is null || !BuscaRecenteFiltroHelper.TemCriterioSalvavel(request.Filtro))
            return new JsonResult(new { sucesso = true });

        await _mediator.Send(new RegistrarBuscaRecenteCommand(usuarioId, request.Filtro));
        return new JsonResult(new { sucesso = true });
    }
}

public class RegistrarBuscaRequest
{
    public ModuloMonolitico.Dominio.Imobiliaria.DTOs.FiltroAnuncio? Filtro { get; set; }
}
