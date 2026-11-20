using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace ModuloMonolitico.Web.TagHelpers;

[HtmlTargetElement("app-input")]
public class AppInputTagHelper : TagHelper
{
    private readonly IHtmlGenerator _generator;

    public AppInputTagHelper(IHtmlGenerator generator)
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

    [HtmlAttributeName("type")]
    public string Type { get; set; } = "text";

    [HtmlAttributeName("label")]
    public string? Label { get; set; }

    [HtmlAttributeName("icon")]
    public string? Icon { get; set; }

    [HtmlAttributeName("placeholder")]
    public string? Placeholder { get; set; }

    [HtmlAttributeName("autocomplete")]
    public string? Autocomplete { get; set; }

    [HtmlAttributeName("class")]
    public string? CssClass { get; set; }

    [HtmlAttributeName("ajuda")]
    public string? Ajuda { get; set; }

    [HtmlAttributeName("mostrar-senha-forte")]
    public bool MostrarSenhaForte { get; set; } = false;

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        var prefix    = ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix;
        string fieldName = For != null ? (string.IsNullOrEmpty(prefix) ? For.Name : $"{prefix}.{For.Name}") : (Name ?? "");
        string fieldId   = For != null ? fieldName.Replace('.', '_') : (Id ?? TagBuilder.CreateSanitizedId(fieldName, "_"));

        string? currentValue = null;
        if (ViewContext.ViewData.ModelState.TryGetValue(fieldName, out var entry) && entry?.AttemptedValue != null)
        {
            currentValue = entry.AttemptedValue;
        }
        else if (For?.Model != null)
        {
            if (Type.Equals("date", StringComparison.OrdinalIgnoreCase) && For.Model is DateTime dt)
                currentValue = dt == default ? null : dt.ToString("yyyy-MM-dd");
            else if (Type.Equals("date", StringComparison.OrdinalIgnoreCase) && For.Model is DateOnly d)
                currentValue = d.ToString("yyyy-MM-dd");
            else
                currentValue = For.Model?.ToString();
        }
        else
        {
            currentValue = Value;
        }

        if (Type.Equals("hidden", StringComparison.OrdinalIgnoreCase))
        {
            var extraAttrsHidden = new StringBuilder();
            foreach (var attr in context.AllAttributes)
            {
                var attrName = attr.Name;
                if (attrName is "asp-for" or "type" or "value" or "name" or "id") continue;
                extraAttrsHidden.Append($" {attrName}=\"{attr.Value}\"");
            }
            var valAttr = currentValue != null ? $" value=\"{System.Net.WebUtility.HtmlEncode(currentValue)}\"" : "";
            output.Content.SetHtmlContent($"<input id=\"{fieldId}\" name=\"{fieldName}\" type=\"hidden\"{valAttr}{extraAttrsHidden}>");
            return;
        }

        if (Type.Equals("radio", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(Label))
        {
            var extraAttrsRadio = new StringBuilder();
            foreach (var attr in context.AllAttributes)
            {
                var attrName = attr.Name;
                if (attrName is "asp-for" or "type" or "value" or "name" or "id" or "checked") continue;
                extraAttrsRadio.Append($" {attrName}=\"{attr.Value}\"");
            }
            var radioVal = Value ?? currentValue ?? "";
            var isChecked = context.AllAttributes.ContainsName("checked") || (currentValue != null && Value != null && currentValue.Equals(Value, StringComparison.OrdinalIgnoreCase));
            var checkedAttr = isChecked ? " checked" : "";
            output.Content.SetHtmlContent($"<input id=\"{fieldId}\" name=\"{fieldName}\" type=\"radio\" value=\"{System.Net.WebUtility.HtmlEncode(radioVal)}\"{checkedAttr}{extraAttrsRadio}>");
            return;
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

        bool isCep = context.AllAttributes.ContainsName("data-mascara") && context.AllAttributes["data-mascara"].Value?.ToString() == "cep";
        bool isDate = Type.Equals("date", StringComparison.OrdinalIgnoreCase);
        bool hasWrapper = !string.IsNullOrEmpty(Icon)
                          || Type.Equals("password", StringComparison.OrdinalIgnoreCase)
                          || isDate
                          || isCep;

        if (hasWrapper)
        {
            if (isDate)
            {
                sb.Append("<div class=\"envoltorio-data\" style=\"display:flex; align-items:stretch; gap:0.5rem;\">");
            }
            else
            {
                string extraClass = isCep ? " cep-wrapper" : "";
                sb.Append($"<div class=\"input-icon-wrapper{extraClass}\" style=\"position:relative;\">");
                if (!string.IsNullOrEmpty(Icon))
                    sb.Append($"<span class=\"input-icon\" style=\"position:absolute; left:1rem; top:50%; transform:translateY(-50%); color:var(--clr-text-muted); pointer-events:none;\"><i class=\"fas {Icon}\"></i></span>");
            }
        }

        var inputStyle = string.IsNullOrEmpty(Icon)
            ? "background:rgba(255,255,255,0.03); border:1px solid var(--glass-border); border-radius:var(--radius-sm); color:var(--clr-text);"
            : "padding-left:2.5rem; background:rgba(255,255,255,0.03); border:1px solid var(--glass-border); border-radius:var(--radius-sm); color:var(--clr-text);";

        var extraAttrs = new StringBuilder();
        foreach (var attr in context.AllAttributes)
        {
            var attrName = attr.Name;
            if (attrName is "asp-for" or "type" or "label" or "icon" or "placeholder" or "autocomplete" or "class" or "style" or "mostrar-senha-forte" or "ajuda" or "name" or "id" or "value")
                continue;
            extraAttrs.Append($" {attrName}=\"{attr.Value}\"");
        }

        var valueAttr = currentValue != null
            ? $" value=\"{System.Net.WebUtility.HtmlEncode(currentValue)}\""
            : "";

        var placeholderAttr = Placeholder != null
            ? $" placeholder=\"{System.Net.WebUtility.HtmlEncode(Placeholder)}\""
            : "";

        var autocompleteAttr = Autocomplete != null
            ? $" autocomplete=\"{Autocomplete}\""
            : "";

        var dateInputStyle = isDate
            ? "flex:1; min-width:0; background:rgba(255,255,255,0.03); border:1px solid var(--glass-border); border-radius:var(--radius-sm); color:var(--clr-text);"
            : inputStyle;

        sb.Append($"<input id=\"{fieldId}\" name=\"{fieldName}\" type=\"{Type}\" class=\"campo-controle\" style=\"{dateInputStyle}\" data-campo-validavel=\"true\"{valueAttr}{placeholderAttr}{autocompleteAttr}{extraAttrs}>");

        if (Type.Equals("password", StringComparison.OrdinalIgnoreCase))
        {
            sb.Append($@"<button type=""button"" data-alternar-senha=""{fieldId}"" aria-label=""Mostrar/ocultar senha"" style=""position:absolute;right:0.75rem;top:50%;transform:translateY(-50%);background:none;border:none;cursor:pointer;color:var(--clr-text-muted);padding:0.25rem;"">
<span class=""icon-eye-open""><svg width=""20"" height=""20"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><path d=""M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z""></path><circle cx=""12"" cy=""12"" r=""3""></circle></svg></span>
<span class=""icon-eye-closed hidden""><svg width=""20"" height=""20"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><path d=""M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24""></path><line x1=""1"" y1=""1"" x2=""23"" y2=""23""></line></svg></span>
</button>");
        }
        else if (isDate)
        {
            sb.Append($@"<button type=""button"" class=""botao-hoje""
                onclick=""var d=new Date(); document.getElementById('{fieldId}').value=d.getFullYear()+'-'+String(d.getMonth()+1).padStart(2,'0')+'-'+String(d.getDate()).padStart(2,'0'); document.getElementById('{fieldId}').dispatchEvent(new Event('change'));""
                title=""Preencher com hoje""
                style=""display:inline-flex;align-items:center;gap:0.3rem;flex-shrink:0;padding:0 0.75rem;height:100%;background:var(--glass-bg);border:1px solid var(--glass-border);border-radius:var(--radius-sm);cursor:pointer;color:var(--clr-text-muted);font-size:0.78rem;font-weight:600;white-space:nowrap;transition:all 150ms ease;"">
                <svg width=""13"" height=""13"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""2"" stroke-linecap=""round""><rect x=""3"" y=""4"" width=""18"" height=""18"" rx=""2"" ry=""2""></rect><line x1=""16"" y1=""2"" x2=""16"" y2=""6""></line><line x1=""8"" y1=""2"" x2=""8"" y2=""6""></line><line x1=""3"" y1=""10"" x2=""21"" y2=""10""></line></svg>
                Hoje
            </button>");
        }
        else if (context.AllAttributes.ContainsName("data-mascara") && context.AllAttributes["data-mascara"].Value?.ToString() == "cep")
        {
            sb.Append($"<button type=\"button\" class=\"botao-buscar-cep\" title=\"Buscar CEP\" style=\"position:absolute;right:0.75rem;top:50%;transform:translateY(-50%);background:var(--clr-primary);border:none;border-radius:4px;cursor:pointer;color:var(--clr-botao-primario-texto);padding:0.25rem 0.6rem;font-size:0.7rem;font-weight:700;display:flex;align-items:center;gap:0.2rem;\"><svg width=\"12\" height=\"12\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"3\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><circle cx=\"11\" cy=\"11\" r=\"8\"></circle><line x1=\"21\" y1=\"21\" x2=\"16.65\" y2=\"16.65\"></line></svg> BUSCAR</button>");
        }

        if (hasWrapper)
            sb.Append("</div>");

        if (Type.Equals("password", StringComparison.OrdinalIgnoreCase) && MostrarSenhaForte)
        {
            sb.Append($@"
<div class=""password-strength-container"" id=""strength-container-{fieldId}"" style=""margin-top: 0.75rem; display: none; background: rgba(255,255,255,0.02); border: 1px solid var(--glass-border); padding: 0.8rem; border-radius: var(--radius-sm);"">
  <div class=""progress"" style=""height: 5px; background: rgba(255,255,255,0.05); border-radius: 4px; overflow: hidden; margin-bottom: 0.5rem;"">
    <div class=""progress-bar"" id=""strength-bar-{fieldId}"" role=""progressbar"" style=""width: 0%; height: 100%; transition: all 0.3s ease; background: #ea4335;""></div>
  </div>
  <div style=""display: flex; justify-content: space-between; align-items: center; font-size: 0.75rem; color: var(--clr-text-muted); margin-bottom: 0.5rem;"">
    <span>Força da senha: <strong id=""strength-text-{fieldId}"" style=""color: #ea4335;"">Fraca</strong></span>
  </div>
  <ul class=""strength-requirements"" style=""list-style: none; padding: 0; margin: 0; font-size: 0.75rem; color: var(--clr-text-muted); display: grid; grid-template-columns: 1fr 1fr; gap: 0.35rem 0.75rem;"">
    <li id=""req-length-{fieldId}"" style=""display: flex; align-items: center; gap: 0.35rem; transition: all 0.2s;"">
      <span class=""req-icon"" style=""display: inline-flex; align-items: center;""><svg width=""12"" height=""12"" viewBox=""0 0 24 24"" fill=""none"" stroke=""var(--clr-text-subtle)"" stroke-width=""3""><circle cx=""12"" cy=""12"" r=""10""></circle></svg></span>
      <span>Mínimo 8 caracteres</span>
    </li>
    <li id=""req-upper-{fieldId}"" style=""display: flex; align-items: center; gap: 0.35rem; transition: all 0.2s;"">
      <span class=""req-icon"" style=""display: inline-flex; align-items: center;""><svg width=""12"" height=""12"" viewBox=""0 0 24 24"" fill=""none"" stroke=""var(--clr-text-subtle)"" stroke-width=""3""><circle cx=""12"" cy=""12"" r=""10""></circle></svg></span>
      <span>Letra maiúscula (A-Z)</span>
    </li>
    <li id=""req-lower-{fieldId}"" style=""display: flex; align-items: center; gap: 0.35rem; transition: all 0.2s;"">
      <span class=""req-icon"" style=""display: inline-flex; align-items: center;""><svg width=""12"" height=""12"" viewBox=""0 0 24 24"" fill=""none"" stroke=""var(--clr-text-subtle)"" stroke-width=""3""><circle cx=""12"" cy=""12"" r=""10""></circle></svg></span>
      <span>Letra minúscula (a-z)</span>
    </li>
    <li id=""req-number-{fieldId}"" style=""display: flex; align-items: center; gap: 0.35rem; transition: all 0.2s;"">
      <span class=""req-icon"" style=""display: inline-flex; align-items: center;""><svg width=""12"" height=""12"" viewBox=""0 0 24 24"" fill=""none"" stroke=""var(--clr-text-subtle)"" stroke-width=""3""><circle cx=""12"" cy=""12"" r=""10""></circle></svg></span>
      <span>Pelo menos um número</span>
    </li>
    <li id=""req-special-{fieldId}"" style=""display: flex; align-items: center; gap: 0.35rem; transition: all 0.2s; grid-column: 1 / -1;"">
      <span class=""req-icon"" style=""display: inline-flex; align-items: center;""><svg width=""12"" height=""12"" viewBox=""0 0 24 24"" fill=""none"" stroke=""var(--clr-text-subtle)"" stroke-width=""3""><circle cx=""12"" cy=""12"" r=""10""></circle></svg></span>
      <span>Caractere especial (ex: @$!%*?&)</span>
    </li>
  </ul>
</div>
");
        }

        if (!string.IsNullOrEmpty(Ajuda))
            sb.Append($"<span class=\"campo-mensagem-ajuda\">{System.Net.WebUtility.HtmlEncode(Ajuda)}</span>");

        if (temErro && erroEntry != null)
        {
            foreach (var error in erroEntry.Errors)
            {
                sb.Append($"<span class=\"campo-mensagem-erro\" data-valmsg-for=\"{fieldName}\" data-valmsg-replace=\"true\"><svg class=\"campo-mensagem-erro-icone\" width=\"12\" height=\"12\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><circle cx=\"12\" cy=\"12\" r=\"10\"></circle><line x1=\"12\" y1=\"8\" x2=\"12\" y2=\"12\"></line><line x1=\"12\" y1=\"16\" x2=\"12.01\" y2=\"16\"></line></svg>{System.Net.WebUtility.HtmlEncode(error.ErrorMessage)}</span>");
            }
        }
        else
        {
            sb.Append($"<span class=\"campo-mensagem-erro\" data-valmsg-for=\"{fieldName}\" data-valmsg-replace=\"true\"></span>");
        }

        sb.Append("</div>");

        output.Content.SetHtmlContent(sb.ToString());
    }
}
