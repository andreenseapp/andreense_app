using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ModuloMonolitico.Web.TagHelpers;

[HtmlTargetElement("app-save-button")]
public class AppSaveButtonTagHelper : TagHelper
{
    [HtmlAttributeName("type")]
    public string Type { get; set; } = "submit";

    [HtmlAttributeName("class")]
    public string? CssClass { get; set; }

    [HtmlAttributeName("full-width")]
    public bool FullWidth { get; set; } = false;

    [HtmlAttributeName("id")]
    public string? Id { get; set; }

    [HtmlAttributeName("style")]
    public string? Style { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "button";
        output.Attributes.SetAttribute("type", Type);
        if (!string.IsNullOrEmpty(Id))
        {
            output.Attributes.SetAttribute("id", Id);
        }

        var baseClass = "botao botao-primario botao-salvar-unificado";
        var baseStyle = "display: flex; align-items: center; justify-content: center; gap: 0.5rem; border-radius: var(--radius-sm); font-weight: 600; cursor: pointer; transition: all var(--transition-fast); background: var(--clr-primary); color: var(--clr-botao-primario-texto); border: none; box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.4);";

        if (FullWidth)
        {
            baseStyle += " width: 100%; padding: 0.8rem;";
        }
        else
        {
            baseStyle += " padding: 0.75rem 1.5rem;";
        }

        if (!string.IsNullOrEmpty(Style))
        {
            baseStyle += " " + Style;
        }

        output.Attributes.SetAttribute("class", $"{baseClass} {CssClass}".Trim());
        output.Attributes.SetAttribute("style", baseStyle);

        var childContent = await output.GetChildContentAsync();
        var text = childContent.GetContent().Trim();
        if (string.IsNullOrEmpty(text))
        {
            text = "Salvar";
        }

        var spinnerHtml = @"<svg class=""spinner-border spinner-border-sm htmx-indicator"" role=""status"" aria-hidden=""true"" style=""display:none; width: 1rem; height: 1rem; margin-right: 0.5rem;"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><line x1=""12"" y1=""2"" x2=""12"" y2=""6""></line><line x1=""12"" y1=""18"" x2=""12"" y2=""22""></line><line x1=""4.93"" y1=""4.93"" x2=""7.76"" y2=""7.76""></line><line x1=""16.24"" y1=""16.24"" x2=""19.07"" y2=""19.07""></line><line x1=""2"" y1=""12"" x2=""6"" y2=""12""></line><line x1=""18"" y1=""12"" x2=""22"" y2=""12""></line><line x1=""4.93"" y1=""19.07"" x2=""7.76"" y2=""16.24""></line><line x1=""16.24"" y1=""7.76"" x2=""19.07"" y2=""4.93""></line></svg>";

        output.Content.SetHtmlContent($"{spinnerHtml}<span class=\"botao-texto\">{text}</span>");
    }
}
