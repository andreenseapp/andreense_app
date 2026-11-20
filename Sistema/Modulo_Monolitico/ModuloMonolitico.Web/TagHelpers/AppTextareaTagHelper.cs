using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace ModuloMonolitico.Web.TagHelpers;

[HtmlTargetElement("app-textarea", Attributes = "asp-for")]
public class AppTextareaTagHelper : TagHelper
{
    private readonly IHtmlGenerator _generator;

    public AppTextareaTagHelper(IHtmlGenerator generator)
    {
        _generator = generator;
    }

    [HtmlAttributeName("asp-for")]
    public ModelExpression For { get; set; } = null!;

    [HtmlAttributeName("label")]
    public string? Label { get; set; }

    [HtmlAttributeName("placeholder")]
    public string? Placeholder { get; set; }

    [HtmlAttributeName("rows")]
    public int Rows { get; set; } = 3;

    [HtmlAttributeName("class")]
    public string? CssClass { get; set; }

    [HtmlAttributeName("ajuda")]
    public string? Ajuda { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        var prefix    = ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix;
        var fieldName = string.IsNullOrEmpty(prefix) ? For.Name : $"{prefix}.{For.Name}";
        var fieldId   = fieldName.Replace('.', '_');

        string? currentValue = null;
        if (ViewContext.ViewData.ModelState.TryGetValue(fieldName, out var entry) && entry?.AttemptedValue != null)
            currentValue = entry.AttemptedValue;
        else
            currentValue = For.Model?.ToString();

        var extraAttrs = new StringBuilder();
        foreach (var attr in context.AllAttributes)
        {
            var name = attr.Name;
            if (name is "asp-for" or "label" or "placeholder" or "rows" or "class" or "style" or "ajuda")
                continue;
            extraAttrs.Append($" {name}=\"{attr.Value}\"");
        }

        var placeholderAttr = Placeholder != null
            ? $" placeholder=\"{System.Net.WebUtility.HtmlEncode(Placeholder)}\""
            : "";

        var sb = new StringBuilder();

        var temErro = ViewContext.ViewData.ModelState.TryGetValue(fieldName, out var erroEntry) && erroEntry.Errors.Count > 0;
        var classeErro = temErro ? " campo-grupo--erro" : "";

        string? wrapperStyle = context.AllAttributes["style"]?.Value?.ToString();
        var styleAttr = string.IsNullOrEmpty(wrapperStyle) ? "" : $" style=\"{System.Net.WebUtility.HtmlEncode(wrapperStyle)}\"";

        sb.Append($"<div class=\"campo-grupo mb-4 {CssClass}{classeErro}\"{styleAttr}>");

        var labelText = Label ?? For.Metadata.DisplayName ?? For.Metadata.PropertyName;
        sb.Append($"<label class=\"campo-rotulo\" for=\"{fieldId}\" style=\"color:var(--clr-text-muted); font-weight:500;\">{labelText}</label>");

        var textContent = currentValue != null ? System.Net.WebUtility.HtmlEncode(currentValue) : "";
        sb.Append($"<textarea id=\"{fieldId}\" name=\"{fieldName}\" rows=\"{Rows}\" class=\"campo-controle\" style=\"background:rgba(255,255,255,0.03); border:1px solid var(--glass-border); border-radius:var(--radius-sm); color:var(--clr-text); resize:vertical;\" data-campo-validavel=\"true\"{placeholderAttr}{extraAttrs}>{textContent}</textarea>");

        if (!string.IsNullOrEmpty(Ajuda))
            sb.Append($"<span class=\"campo-mensagem-ajuda\">{System.Net.WebUtility.HtmlEncode(Ajuda)}</span>");

        if (temErro && erroEntry != null)
        {
            foreach (var error in erroEntry.Errors)
                sb.Append($"<span class=\"campo-mensagem-erro\" data-valmsg-for=\"{fieldName}\" data-valmsg-replace=\"true\"><svg class=\"campo-mensagem-erro-icone\" width=\"12\" height=\"12\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><circle cx=\"12\" cy=\"12\" r=\"10\"></circle><line x1=\"12\" y1=\"8\" x2=\"12\" y2=\"12\"></line><line x1=\"12\" y1=\"16\" x2=\"12.01\" y2=\"16\"></line></svg>{System.Net.WebUtility.HtmlEncode(error.ErrorMessage)}</span>");
        }
        else
        {
            sb.Append($"<span class=\"campo-mensagem-erro\" data-valmsg-for=\"{fieldName}\" data-valmsg-replace=\"true\"></span>");
        }

        sb.Append("</div>");

        output.Content.SetHtmlContent(sb.ToString());
    }
}
