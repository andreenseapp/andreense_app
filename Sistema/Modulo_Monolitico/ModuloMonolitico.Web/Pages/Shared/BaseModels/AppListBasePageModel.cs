using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Dominio.Configuracoes;

namespace ModuloMonolitico.Web.Pages.Shared;

public interface IListPagination
{
    int PaginaAtual { get; }
    int TotalPaginas { get; }
}

public abstract class AppListBasePageModel<TItem, TFiltro> : AppBasePageModel, IListPagination
    where TFiltro : class, new()
{
    [BindProperty(SupportsGet = true)]
    public TFiltro Filtros { get; set; } = new TFiltro();

    [BindProperty(SupportsGet = true)]
    public int PaginaAtual { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public string? OrdenarPor { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? OrdenarDirecao { get; set; } = "desc";

    public int TamanhoPagina { get; protected set; } = 30;
    public int TotalItens { get; protected set; }
    public int TotalPaginas => (int)Math.Ceiling(TotalItens / (double)TamanhoPagina);

    public IEnumerable<TItem> Itens { get; protected set; } = [];

    [BindProperty]
    public List<Guid> IdsSelecionados { get; set; } = new();

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    protected IActionResult? RestaurarOuSalvarFiltros(string cacheKey)
    {
        var cookieKey = ChavesSistema.CookieFiltros(cacheKey, UsuarioId);

        bool temFiltrosNaQuery = Request.Query.Keys.Any(k => !k.Equals("PaginaAtual", StringComparison.OrdinalIgnoreCase) && !k.Equals("handler", StringComparison.OrdinalIgnoreCase));

        if (temFiltrosNaQuery)
        {
            var jsonFiltros = JsonSerializer.Serialize(Filtros);
            Response.Cookies.Append(cookieKey, jsonFiltros, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(7) });
            return null;
        }
        else
        {
            var jsonSalvo = Request.Cookies[cookieKey];
            if (!string.IsNullOrEmpty(jsonSalvo))
            {
                try
                {
                    var filtrosRecuperados = JsonSerializer.Deserialize<TFiltro>(jsonSalvo);
                    if (filtrosRecuperados != null)
                    {
                        Filtros = filtrosRecuperados;
                    }
                }
                catch
                {
                }
            }
        }

        return null;
    }

    public string ObterDirecaoOrdenacaoAlternada(string campo)
    {
        if (string.Equals(OrdenarPor, campo, StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(OrdenarDirecao, "asc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
        }
        return "asc";
    }

    public string RenderizarIconeOrdenacao(string campo)
    {
        if (!string.Equals(OrdenarPor, campo, StringComparison.OrdinalIgnoreCase))
        {
            return "<span class=\"sort-icon\" style=\"opacity: 0.4; font-size: 11px;\"> ⇅</span>";
        }

        var icon = string.Equals(OrdenarDirecao, "asc", StringComparison.OrdinalIgnoreCase) ? " ▲" : " ▼";
        return $"<span class=\"sort-icon\" style=\"opacity: 1; font-size: 11px;\">{icon}</span>";
    }
}

public class FloatingBulkActionsViewModel
{
    public string FormId { get; set; } = "";
    public string Handler { get; set; } = "";
    public string TextoBotao { get; set; } = "Excluir Selecionados";
    public string MensagemConfirmacao { get; set; } = "Deseja realmente aplicar esta ação aos itens selecionados?";
}
