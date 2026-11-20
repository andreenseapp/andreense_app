using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace ModuloMonolitico.Web.TagHelpers;

[HtmlTargetElement("app-select")]
public class AppSelectTagHelper : TagHelper
{
    private readonly IHtmlGenerator _generator;

    public AppSelectTagHelper(IHtmlGenerator generator)
    {
        _generator = generator;
    }

    [HtmlAttributeName("asp-for")]
    public ModelExpression? For { get; set; }

    [HtmlAttributeName("name")]
    public string? Name { get; set; }

    [HtmlAttributeName("id")]
    public string? Id { get; set; }

    [HtmlAttributeName("value")]
    public string? Value { get; set; }

    [HtmlAttributeName("asp-items")]
    public IEnumerable<SelectListItem>? Items { get; set; }

    [HtmlAttributeName("label")]
    public string? Label { get; set; }

    [HtmlAttributeName("class")]
    public string? CssClass { get; set; }

    [HtmlAttributeName("ajuda")]
    public string? Ajuda { get; set; }

    [HtmlAttributeName("default-option")]
    public string? DefaultOption { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        var prefix    = ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix;
        string fieldName = For != null ? (string.IsNullOrEmpty(prefix) ? For.Name : $"{prefix}.{For.Name}") : (Name ?? "");
        string fieldId   = For != null ? fieldName.Replace('.', '_') : (Id ?? TagBuilder.CreateSanitizedId(fieldName, "_"));

        string? currentValue = null;
        if (ViewContext.ViewData.ModelState.TryGetValue(fieldName, out var entry) && entry?.AttemptedValue != null)
            currentValue = entry.AttemptedValue;
        else if (For != null)
            currentValue = For.Model?.ToString();
        else
            currentValue = Value;

        var childContent = await output.GetChildContentAsync();
        var optionsHtml  = childContent.GetContent();

        if (Items != null)
        {
            var itemsSb = new StringBuilder();
            if (!string.IsNullOrEmpty(DefaultOption))
                itemsSb.Append($"<option value=\"\">{System.Net.WebUtility.HtmlEncode(DefaultOption)}</option>");

            foreach (var item in Items)
            {
                var sel      = (item.Value == currentValue) ? " selected" : "";
                var disabled = item.Disabled ? " disabled" : "";
                itemsSb.Append($"<option value=\"{System.Net.WebUtility.HtmlEncode(item.Value)}\"{sel}{disabled}>{System.Net.WebUtility.HtmlEncode(item.Text)}</option>");
            }
            optionsHtml = itemsSb.ToString();
        }
        else if (!string.IsNullOrEmpty(currentValue) && !string.IsNullOrEmpty(optionsHtml))
        {
            optionsHtml = System.Text.RegularExpressions.Regex.Replace(
                optionsHtml,
                $"(<option[^>]*value=\"{System.Text.RegularExpressions.Regex.Escape(currentValue)}\"[^>]*)>",
                "$1 selected>");
        }

        var sb = new StringBuilder();

        var temErro = ViewContext.ViewData.ModelState.TryGetValue(fieldName, out var erroEntry) && erroEntry.Errors.Count > 0;
        var classeErro = temErro ? " campo-grupo--erro" : "";

        string? wrapperStyle = context.AllAttributes["style"]?.Value?.ToString();
        var styleAttr = string.IsNullOrEmpty(wrapperStyle) ? "" : $" style=\"{System.Net.WebUtility.HtmlEncode(wrapperStyle)}\"";

        sb.Append($"<div class=\"campo-grupo mb-4 {CssClass}{classeErro}\"{styleAttr}>");

        var labelText = Label ?? (For != null ? (For.Metadata.DisplayName ?? For.Metadata.PropertyName) : Name);
        if (!string.IsNullOrEmpty(labelText))
        {
            sb.Append($"<label class=\"campo-rotulo\" for=\"{fieldId}\" style=\"color:var(--clr-text-muted); font-weight:500;\">{labelText}</label>");
        }

        var extraAttrs = new StringBuilder();
        foreach (var attr in context.AllAttributes)
        {
            var attrName = attr.Name;
            if (attrName is "asp-for" or "asp-items" or "label" or "class" or "default-option" or "style" or "ajuda" or "name" or "id" or "value")
                continue;
            extraAttrs.Append($" {attrName}=\"{attr.Value}\"");
        }

        sb.Append($"<select id=\"{fieldId}\" name=\"{fieldName}\" class=\"campo-controle\" style=\"background:rgba(255,255,255,0.03); border:1px solid var(--glass-border); border-radius:var(--radius-sm); color:var(--clr-text);\" data-campo-validavel=\"true\"{extraAttrs}>");
        sb.Append(optionsHtml);
        sb.Append("</select>");

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
