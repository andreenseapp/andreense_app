using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ModuloMonolitico.Dominio.Configuracoes;

namespace ModuloMonolitico.Web.TagHelpers;

[HtmlTargetElement("app-toggle", Attributes = "asp-for")]
public class AppToggleTagHelper : TagHelper
{
    private readonly IHtmlGenerator _generator;

    public AppToggleTagHelper(IHtmlGenerator generator)
    {
        _generator = generator;
    }

    [HtmlAttributeName("asp-for")]
    public ModelExpression For { get; set; } = null!;

    [HtmlAttributeName("label-on")]
    public string LabelOn { get; set; } = "Sim";

    [HtmlAttributeName("label-off")]
    public string LabelOff { get; set; } = "Não";

    [HtmlAttributeName("value-on")]
    public string ValueOn { get; set; } = "S";

    [HtmlAttributeName("value-off")]
    public string ValueOff { get; set; } = "N";

    [HtmlAttributeName("class")]
    public string? CssClass { get; set; }

    [HtmlAttributeName("title")]
    public string? Title { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        
        bool isBool = For.ModelExplorer.ModelType == typeof(bool) || For.ModelExplorer.ModelType == typeof(bool?);
        
        var contentBuilder = new System.Text.StringBuilder();
        contentBuilder.Append($"<div class=\"app-toggle-wrapper {CssClass}\">");

        var displayName = Title ?? For.Metadata.DisplayName ?? For.Metadata.PropertyName;
        if (!string.IsNullOrEmpty(displayName))
        {
            contentBuilder.Append($"<span class=\"app-toggle-title\">{displayName}</span>");
        }

        contentBuilder.Append("<label class=\"app-toggle-container\">");

        if (isBool)
        {
            var inputAttributes = new Dictionary<string, object>
            {
                { "class", "app-toggle-input" }
            };

            foreach (var attr in context.AllAttributes)
            {
                var name = attr.Name;
                if (name != "asp-for" && name != "label-on" && name != "label-off" && name != "value-on" && name != "value-off" && name != "class")
                {
                    inputAttributes[name] = attr.Value;
                }
            }

            var inputTag = _generator.GenerateCheckBox(ViewContext, For.ModelExplorer, For.Name, null, inputAttributes);
            using (var writer = new System.IO.StringWriter())
            {
                inputTag?.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                contentBuilder.Append(writer.ToString());
            }
        }
        else
        {
            var isChecked = For.Model?.ToString() == ValueOn;
            var hiddenId = ChavesSistema.HtmlHiddenId(For.Name);
            
            contentBuilder.Append($"<input type=\"hidden\" name=\"{For.Name}\" id=\"{hiddenId}\" value=\"{(isChecked ? ValueOn : ValueOff)}\" />");
            
            contentBuilder.Append($"<input type=\"checkbox\" class=\"app-toggle-input\" {(isChecked ? "checked" : "")} onchange=\"document.getElementById('{hiddenId}').value = this.checked ? '{ValueOn}' : '{ValueOff}';\" />");
        }

        contentBuilder.Append($"<span class=\"app-toggle-slider\"></span>");
        contentBuilder.Append($"<span class=\"app-toggle-label app-toggle-label-on\">{LabelOn}</span>");
        contentBuilder.Append($"<span class=\"app-toggle-label app-toggle-label-off\">{LabelOff}</span>");
        
        contentBuilder.Append("</label>");
        contentBuilder.Append("</div>");

        output.Content.SetHtmlContent(contentBuilder.ToString());
    }
}
