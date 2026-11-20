using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Consultas;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.Imobiliaria.Servicos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Servicos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Imoveis;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IAmenidadeRepositorio _amenidadeRepositorio;
    private readonly IServicoIdentidadeJwt _identidade;
    private readonly IRegistradorDesempenhoRequisicao _desempenho;

    public IndexModel(
        IMediator mediator,
        IAmenidadeRepositorio amenidadeRepositorio,
        IServicoIdentidadeJwt identidade,
        IRegistradorDesempenhoRequisicao desempenho)
    {
        _mediator = mediator;
        _amenidadeRepositorio = amenidadeRepositorio;
        _identidade = identidade;
        _desempenho = desempenho;
    }

    public ResultadoPaginado<AnuncioResumoDto> Resultado { get; set; } = new();
    public IReadOnlyList<AnuncioResumoDto> ItensGrade { get; set; } = [];
    public IReadOnlyList<AmenidadeDto> CatalogoAmenidades { get; set; } = [];
    public bool UsuarioLogado { get; set; }
    public bool UsuarioEhMaster { get; set; }
    public bool TemBuscaSalvavel { get; set; }
    public string ChaveFiltroAtual { get; set; } = string.Empty;
    public string RotuloBuscaAtual { get; set; } = string.Empty;
    public string ChaveStorageBuscasRecentes { get; set; } = ChavesSistema.StorageLocalBuscasRecentesImoveis();
    public string UrlBuscasRecentes { get; set; } = ModuloRotas.Imoveis.BuscasRecentes;
    public string UrlFavoritos { get; set; } = ModuloRotas.Imoveis.Favoritos;
    public string UrlImoveis { get; set; } = ModuloRotas.Imoveis.Index;
    public string UrlContextoUsuario { get; set; } = ModuloRotas.Imoveis.ContextoUsuario;
    public string UrlAlternarDestaque { get; set; } = ModuloRotas.Imoveis.AlternarDestaque;
    public IReadOnlyList<AnuncioResumoDto> AnunciosDestaque { get; set; } = [];
    public string ChaveStorageVisualizados { get; set; } = ChavesSistema.StorageLocalAnunciosVisualizadosImoveis();
    public string UrlRegistrarVisualizacao { get; set; } = ModuloRotas.Imoveis.RegistrarVisualizacao;

    [BindProperty(SupportsGet = true)]
    public FiltroAnuncio Filtro { get; set; } = new();

    public bool FiltrosAtivos =>
        !string.IsNullOrWhiteSpace(Filtro.TipoImovel) ||
        !string.IsNullOrWhiteSpace(Filtro.Bairro) ||
        Filtro.QuartosMinimo.HasValue ||
        Filtro.BanheirosMinimo.HasValue ||
        Filtro.VagasMinimo.HasValue ||
        Filtro.ValorMinimo.HasValue ||
        Filtro.ValorMaximo.HasValue ||
        Filtro.ValorCondominioMinimo.HasValue ||
        Filtro.ValorCondominioMaximo.HasValue ||
        Filtro.AreaMinima.HasValue ||
        Filtro.AreaMaxima.HasValue ||
        !string.IsNullOrWhiteSpace(Filtro.SituacaoObra) ||
        !string.IsNullOrWhiteSpace(Filtro.CodigoPublico) ||
        (Filtro.CodigosAmenidade?.Count > 0) ||
        Filtro.BuscarPorPacote ||
        string.Equals(Filtro.TipoNegocio, "aluguel", System.StringComparison.OrdinalIgnoreCase);

    public string QueryStringFiltros
    {
        get
        {
            var qs = BuscaRecenteFiltroHelper.MontarQueryString(Filtro);
            return string.IsNullOrEmpty(qs) ? string.Empty : "&" + qs;
        }
    }

    public async Task<IActionResult> OnGetAsync([FromQuery] int pagina = 1)
    {
        if (string.IsNullOrWhiteSpace(Filtro.TipoNegocio))
            Filtro.TipoNegocio = "venda";

        Filtro.Pagina = pagina;
        UsuarioLogado = _identidade.TentarResolverUsuarioId(User, out var usuarioId);
        UsuarioEhMaster = UsuarioLogado
            && _identidade.EhUsuarioMaster(usuarioId, _identidade.ObterEmail(User));

        TemBuscaSalvavel = BuscaRecenteFiltroHelper.TemCriterioSalvavel(Filtro);
        ChaveFiltroAtual = TemBuscaSalvavel
            ? BuscaRecenteFiltroHelper.GerarChave(Filtro)
            : string.Empty;
        RotuloBuscaAtual = TemBuscaSalvavel
            ? BuscaRecenteFiltroHelper.GerarRotulo(Filtro)
            : string.Empty;

        var catalogoTask = _desempenho.MedirAsync(
            "catalogo_amenidades",
            () => _amenidadeRepositorio.ListarTodasAsync());

        var resultadoTask = _desempenho.MedirAsync(
            "listagem_anuncios",
            () => _mediator.Send(new ListarAnunciosQuery(Filtro)));

        await Task.WhenAll(catalogoTask, resultadoTask);

        CatalogoAmenidades = await catalogoTask;
        Resultado = await resultadoTask;

        if (pagina == 1)
        {
            AnunciosDestaque = Resultado.Itens.Where(a => a.EhDestaque).Take(6).ToList();
            var idsDestaque = AnunciosDestaque.Select(a => a.AnuncioId).ToHashSet();
            ItensGrade = idsDestaque.Count > 0
                ? Resultado.Itens.Where(i => !idsDestaque.Contains(i.AnuncioId)).ToList()
                : Resultado.Itens.ToList();
        }
        else
        {
            ItensGrade = Resultado.Itens.ToList();
        }

        return Page();
    }
}
