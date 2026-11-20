using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ModuloMonolitico.Web.TagHelpers;

[HtmlTargetElement("app-checkbox")]
public class AppCheckboxTagHelper : TagHelper
{
    private readonly IHtmlGenerator _generator;

    public AppCheckboxTagHelper(IHtmlGenerator generator)
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

    [HtmlAttributeName("checked")]
    public bool Checked { get; set; } = false;

    [HtmlAttributeName("label")]
    public string? Label { get; set; }

    [HtmlAttributeName("class")]
    public string? CssClass { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        var childContent = await output.GetChildContentAsync();
        var innerHtml = childContent.GetContent();

        var contentBuilder = new System.Text.StringBuilder();
        contentBuilder.Append($"<div class=\"campo-check mb-3 {CssClass}\" style=\"display: flex; align-items: center; gap: 0.5rem;\">");

        var inputAttributes = new Dictionary<string, object>
        {
            { "class", "campo-check-input" },
            { "style", "cursor: pointer; width: 1.2rem; height: 1.2rem; margin-top: 0; background-color: rgba(255,255,255,0.05); border: 1px solid var(--glass-border);" }
        };

        foreach (var attr in context.AllAttributes)
        {
            var attrName = attr.Name;
            if (attrName != "asp-for" && attrName != "label" && attrName != "class" && attrName != "name" && attrName != "id" && attrName != "value" && attrName != "checked")
            {
                inputAttributes[attrName] = attr.Value;
            }
        }

        string fieldName = For != null ? (string.IsNullOrEmpty(ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix) ? For.Name : $"{ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix}.{For.Name}") : (Name ?? "");
        string fieldId = For != null ? fieldName.Replace('.', '_') : (Id ?? TagBuilder.CreateSanitizedId(fieldName, "_"));
        var labelText = Label ?? (For != null ? (For.Metadata.DisplayName ?? For.Metadata.PropertyName) : Name);

        if (For != null)
        {
            var inputTag = _generator.GenerateCheckBox(ViewContext, For.ModelExplorer, For.Name, null, inputAttributes);
            using (var writer = new System.IO.StringWriter())
            {
                inputTag?.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                contentBuilder.Append(writer.ToString());
            }
        }
        else
        {
            var tag = new TagBuilder("input");
            tag.Attributes["type"] = "checkbox";
            tag.Attributes["name"] = fieldName;
            tag.Attributes["id"] = fieldId;
            tag.Attributes["value"] = Value ?? "true";
            if (Checked || (context.AllAttributes.ContainsName("checked") && context.AllAttributes["checked"].Value?.ToString().ToLowerInvariant() != "false"))
            {
                tag.Attributes["checked"] = "checked";
            }
            foreach (var kvp in inputAttributes)
            {
                tag.Attributes[kvp.Key] = kvp.Value.ToString();
            }
            using (var writer = new System.IO.StringWriter())
            {
                tag.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                contentBuilder.Append(writer.ToString());
            }
        }

        if (!string.IsNullOrEmpty(innerHtml) && string.IsNullOrEmpty(Label))
        {
            contentBuilder.Append($"<label class=\"campo-check-rotulo\" for=\"{fieldId}\" style=\"cursor: pointer; color: var(--clr-text-muted); font-weight: 500; padding-top: 2px;\">");
            contentBuilder.Append(innerHtml);
            contentBuilder.Append("</label>");
        }
        else if (!string.IsNullOrEmpty(labelText))
        {
            if (For != null)
            {
                var labelTag = _generator.GenerateLabel(ViewContext, For.ModelExplorer, For.Name, labelText, new { @class = "campo-check-rotulo", style = "cursor: pointer; color: var(--clr-text-muted); font-weight: 500; padding-top: 2px;" });
                using (var writer = new System.IO.StringWriter())
                {
                    labelTag?.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                    contentBuilder.Append(writer.ToString());
                }
            }
            else
            {
                contentBuilder.Append($"<label class=\"campo-check-rotulo\" for=\"{fieldId}\" style=\"cursor: pointer; color: var(--clr-text-muted); font-weight: 500; padding-top: 2px;\">{System.Net.WebUtility.HtmlEncode(labelText)}</label>");
            }
        }

        contentBuilder.Append("</div>");

        output.Content.SetHtmlContent(contentBuilder.ToString());
    }
}

