using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Dominio.InterfacesDeServico;

namespace ModuloMonolitico.Web.Pages.Shared.Components.PainelDesempenho;

public class PainelDesempenhoViewComponent : ViewComponent
{
    private readonly IRegistradorDesempenhoRequisicao _registrador;

    public PainelDesempenhoViewComponent(IRegistradorDesempenhoRequisicao registrador)
    {
        _registrador = registrador;
    }

    public IViewComponentResult Invoke()
    {
        if (!_registrador.EstaHabilitado)
            return Content(string.Empty);

        var modelo = new PainelDesempenhoModelo
        {
            Etapas = _registrador.ObterEtapas()
                .OrderByDescending(e => e.DuracaoMs)
                .ToList(),
            Caminho = HttpContext.Request.Path + HttpContext.Request.QueryString,
            Metodo = HttpContext.Request.Method
        };

        return View(modelo);
    }
}

public class PainelDesempenhoModelo
{
    public IReadOnlyList<ModuloMonolitico.Dominio.Compartilhado.MedicaoEtapaDesempenho> Etapas { get; set; } = [];
    public string Caminho { get; set; } = string.Empty;
    public string Metodo { get; set; } = string.Empty;

    public long DuracaoTotalMs =>
        Etapas.FirstOrDefault(e => e.Nome.Equals("requisicao_total", StringComparison.OrdinalIgnoreCase))?.DuracaoMs ?? 0;

    public long DuracaoMaiorEtapaMs =>
        Etapas.Where(e => !e.Nome.Equals("requisicao_total", StringComparison.OrdinalIgnoreCase))
            .Select(e => e.DuracaoMs)
            .DefaultIfEmpty(0)
            .Max();
}
