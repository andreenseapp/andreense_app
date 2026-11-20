using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Mensagens;

public class IndexModel : AppBasePageModel
{
    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;

    public List<TopicoInboxDto> Topicos { get; private set; } = new();
    public List<MensagemDto> Mensagens { get; private set; } = new();
    public Guid? TopicoAtivoId { get; private set; }
    public TopicoInboxDto? TopicoAtivo { get; private set; }

    [BindProperty] public Guid TopicoIdInput { get; set; }
    [BindProperty] public string CorpoTexto { get; set; } = string.Empty;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnGetAsync(Guid? topicoId)
    {
        Topicos = (await _mediator.Send(new ListarTopicosMensagensQuery(UsuarioId))).ToList();

        if (!topicoId.HasValue) return;

        var resultado = await _mediator.Send(new ObterMensagensTopicoQuery(topicoId.Value, UsuarioId));
        if (resultado == null) return;

        TopicoAtivoId = topicoId;
        TopicoAtivo = resultado.TopicoAtivo;
        Mensagens = resultado.Mensagens.ToList();
        Topicos = resultado.Topicos.ToList();
    }

    public async Task<IActionResult> OnPostEnviarAsync()
    {
        if (string.IsNullOrWhiteSpace(CorpoTexto) || TopicoIdInput == Guid.Empty)
            return RedirectToPage(new { topicoId = TopicoIdInput });

        var enviado = await _mediator.Send(new EnviarMensagemTopicoCommand(TopicoIdInput, UsuarioId, CorpoTexto));
        if (!enviado)
            return Forbid();

        Logger.LogInformation("Usuario {UsuarioId} enviou uma mensagem no topico {TopicoId}.", UsuarioId, TopicoIdInput);

        return RedirectToPage(new { topicoId = TopicoIdInput });
    }

    public string RenderizarCorpoTextoSeguro(string textoOriginal)
    {
        if (string.IsNullOrWhiteSpace(textoOriginal)) return string.Empty;

        var htmlEncoded = System.Text.Encodings.Web.HtmlEncoder.Default.Encode(textoOriginal);
        var pattern = @"\[(.*?)\]\((.*?)\)";
        return System.Text.RegularExpressions.Regex.Replace(
            htmlEncoded,
            pattern,
            m => $"<a href=\"{m.Groups[2].Value}\" class=\"msg-link\">{m.Groups[1].Value}</a>");
    }
}
