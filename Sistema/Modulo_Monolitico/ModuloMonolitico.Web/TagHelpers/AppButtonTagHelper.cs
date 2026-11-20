using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ModuloMonolitico.Web.TagHelpers;

[HtmlTargetElement("app-button")]
public class AppButtonTagHelper : TagHelper
{
    [HtmlAttributeName("variant")]
    public string Variant { get; set; } = "primary";

    [HtmlAttributeName("type")]
    public string Type { get; set; } = "button";

    [HtmlAttributeName("class")]
    public string? CssClass { get; set; }

    [HtmlAttributeName("full-width")]
    public bool FullWidth { get; set; } = false;

    [HtmlAttributeName("disabled")]
    public bool Disabled { get; set; } = false;

    [HtmlAttributeName("size")]
    public string? Size { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "button";
        output.Attributes.SetAttribute("type", Type);

        if (Disabled)
        {
            output.Attributes.SetAttribute("disabled", "disabled");
        }

        var baseClass = "botao";
        var style = "display: flex; align-items: center; justify-content: center; gap: 0.5rem; border-radius: var(--radius-sm); font-weight: 600; cursor: pointer; transition: all var(--transition-fast);";

        switch (Variant.ToLower())
        {
            case "primary":
                baseClass += " botao-primario";
                style += " background: var(--clr-primary); color: var(--clr-botao-primario-texto); border: none; box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.4);";
                break;
            case "secondary":
                baseClass += " botao-secundario";
                style += " background: var(--glass-bg); color: var(--clr-text); border: 1px solid var(--glass-border);";
                break;
            case "ghost":
                baseClass += " botao-fantasma";
                style += " background: transparent; color: var(--clr-text); border: none;";
                break;
            case "danger":
                baseClass += " botao-perigo";
                style += " background: rgba(239, 68, 68, 0.1); color: #ef4444; border: 1px solid rgba(239, 68, 68, 0.2);";
                break;
            default:
                baseClass += " botao-primario";
                break;
        }

        if (FullWidth)
        {
            style += " width: 100%; padding: 0.8rem;";
        }
        else if (!string.IsNullOrEmpty(Size) && Size.ToLower() == "sm")
        {
            style += " padding: 0.35rem 0.75rem; font-size: 0.825rem;";
        }
        else if (!string.IsNullOrEmpty(Size) && Size.ToLower() == "lg")
        {
            style += " padding: 1rem 2rem; font-size: 1.125rem;";
        }
        else
        {
            style += " padding: 0.75rem 1.5rem;";
        }

        output.Attributes.SetAttribute("class", $"{baseClass} {CssClass}".Trim());
        output.Attributes.SetAttribute("style", style);

        if (!context.AllAttributes.ContainsName("hx-indicator") && context.AllAttributes.ContainsName("hx-post"))
        {
        }

        var childContent = await output.GetChildContentAsync();
        
        var spinnerHtml = @"<svg class=""spinner-border spinner-border-sm htmx-indicator"" role=""status"" aria-hidden=""true"" style=""display:none; width: 1rem; height: 1rem; margin-right: 0.5rem;"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><line x1=""12"" y1=""2"" x2=""12"" y2=""6""></line><line x1=""12"" y1=""18"" x2=""12"" y2=""22""></line><line x1=""4.93"" y1=""4.93"" x2=""7.76"" y2=""7.76""></line><line x1=""16.24"" y1=""16.24"" x2=""19.07"" y2=""19.07""></line><line x1=""2"" y1=""12"" x2=""6"" y2=""12""></line><line x1=""18"" y1=""12"" x2=""22"" y2=""12""></line><line x1=""4.93"" y1=""19.07"" x2=""7.76"" y2=""16.24""></line><line x1=""16.24"" y1=""7.76"" x2=""19.07"" y2=""4.93""></line></svg>";

        output.Content.SetHtmlContent($"{spinnerHtml}<span class=\"botao-texto\">{childContent.GetContent()}</span>");
    }
}

