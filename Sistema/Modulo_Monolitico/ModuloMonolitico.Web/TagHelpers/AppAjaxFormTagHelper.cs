using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ModuloMonolitico.Web.TagHelpers;

[HtmlTargetElement("form", Attributes = "app-ajax")]
public class AppAjaxFormTagHelper : TagHelper
{
    [HtmlAttributeName("app-ajax-botao")]
    public string? BotaoSeletor { get; set; }

    [HtmlAttributeName("app-ajax-erro")]
    public string? MensagemErro { get; set; }

    [HtmlAttributeName("app-ajax-titulo-erro")]
    public string? TituloErro { get; set; }

    [HtmlAttributeName("app-ajax-modo-erro")]
    public string ModoErro { get; set; } = "toast";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.SetAttribute("data-app-ajax-form", "true");

        if (!string.IsNullOrWhiteSpace(BotaoSeletor))
            output.Attributes.SetAttribute("data-app-ajax-botao", BotaoSeletor);

        if (!string.IsNullOrWhiteSpace(MensagemErro))
            output.Attributes.SetAttribute("data-app-ajax-erro", MensagemErro);

        if (!string.IsNullOrWhiteSpace(TituloErro))
            output.Attributes.SetAttribute("data-app-ajax-titulo-erro", TituloErro);

        if (!string.IsNullOrWhiteSpace(ModoErro))
            output.Attributes.SetAttribute("data-app-ajax-modo-erro", ModoErro);
    }
}
