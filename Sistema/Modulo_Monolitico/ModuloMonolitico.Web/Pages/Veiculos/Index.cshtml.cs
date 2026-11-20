using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Consultas;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using ModuloMonolitico.Dominio.Veiculos.DTOs;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Servicos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Veiculos;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IServicoIdentidadeJwt _identidade;

    public IndexModel(IMediator mediator, IServicoIdentidadeJwt identidade)
    {
        _mediator = mediator;
        _identidade = identidade;
    }

    public ResultadoPaginado<AnuncioVeiculoResumoDto> Resultado { get; set; } = new();
    public IReadOnlyList<AnuncioVeiculoResumoDto> ItensGrade { get; set; } = [];
    public IReadOnlyList<AnuncioVeiculoResumoDto> AnunciosDestaque { get; set; } = [];
    public bool UsuarioLogado { get; set; }
    public bool UsuarioEhMaster { get; set; }
    public string ChaveStorageBuscasRecentes { get; set; } = ChavesSistema.StorageLocalBuscasRecentesVeiculos();
    public string UrlBuscasRecentes { get; set; } = ModuloRotas.Veiculos.BuscasRecentes;
    public string UrlFavoritos { get; set; } = ModuloRotas.Veiculos.Favoritos;
    public string UrlVeiculos { get; set; } = ModuloRotas.Veiculos.Index;
    public string UrlContextoUsuario { get; set; } = ModuloRotas.Veiculos.ContextoUsuario;
    public string UrlAlternarDestaque { get; set; } = ModuloRotas.Veiculos.AlternarDestaque;

    [BindProperty(SupportsGet = true)]
    public FiltroAnuncioVeiculo Filtro { get; set; } = new();

    public bool FiltrosAtivos =>
        !string.IsNullOrWhiteSpace(Filtro.Marca) ||
        !string.IsNullOrWhiteSpace(Filtro.Modelo) ||
        !string.IsNullOrWhiteSpace(Filtro.TipoVeiculo) ||
        !string.IsNullOrWhiteSpace(Filtro.Combustivel) ||
        !string.IsNullOrWhiteSpace(Filtro.Cambio) ||
        Filtro.AnoMinimo.HasValue ||
        Filtro.AnoMaximo.HasValue ||
        Filtro.KmMaximo.HasValue ||
        Filtro.PrecoMinimo.HasValue ||
        Filtro.PrecoMaximo.HasValue ||
        Filtro.AceitaTroca.HasValue ||
        !string.IsNullOrWhiteSpace(Filtro.CodigoPublico);

    public async Task<IActionResult> OnGetAsync([FromQuery] int pagina = 1)
    {
        Filtro.Pagina = pagina;
        UsuarioLogado = _identidade.TentarResolverUsuarioId(User, out var usuarioId);
        UsuarioEhMaster = UsuarioLogado && _identidade.EhUsuarioMaster(usuarioId, _identidade.ObterEmail(User));

        Resultado = await _mediator.Send(new ListarAnunciosVeiculosQuery(Filtro));

        if (pagina == 1)
        {
            AnunciosDestaque = Resultado.Itens.Where(a => a.EhDestaque).Take(6).ToList();
            var idsDestaque = AnunciosDestaque.Select(a => a.AnuncioVeiculoId).ToHashSet();
            ItensGrade = idsDestaque.Count > 0
                ? Resultado.Itens.Where(i => !idsDestaque.Contains(i.AnuncioVeiculoId)).ToList()
                : Resultado.Itens.ToList();
        }
        else
        {
            ItensGrade = Resultado.Itens.ToList();
        }

        return Page();
    }
}
