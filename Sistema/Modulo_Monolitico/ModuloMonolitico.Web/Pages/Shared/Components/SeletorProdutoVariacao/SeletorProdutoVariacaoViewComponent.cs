using Microsoft.AspNetCore.Mvc;

namespace ModuloMonolitico.Web.Pages.Shared.Components.SeletorProdutoVariacao;

public class SeletorProdutoVariacaoViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        string label = "Serviço / Produto",
        string fieldName = "ProdutoVariacaoId",
        Guid? valorAtual = null,
        string? nomeAtual = null,
        string? codigoAtual = null,
        bool apenasServicos = false,
        bool desabilitado = false)
    {
        var vm = new SeletorProdutoVariacaoViewModel
        {
            Label = label,
            FieldName = fieldName,
            ValorAtual = valorAtual,
            NomeAtual = nomeAtual,
            CodigoAtual = codigoAtual,
            ApenasServicos = apenasServicos,
            Desabilitado = desabilitado,
            ComponentId = $"spv_{Guid.NewGuid():N}"
        };

        return View(vm);
    }
}

public class SeletorProdutoVariacaoViewModel
{
    public string Label { get; set; } = "Serviço / Produto";
    public string FieldName { get; set; } = "ProdutoVariacaoId";
    public Guid? ValorAtual { get; set; }
    public string? NomeAtual { get; set; }
    public string? CodigoAtual { get; set; }
    public bool ApenasServicos { get; set; }
    public bool Desabilitado { get; set; }
    public string ComponentId { get; set; } = string.Empty;
}
