using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Cidadania.AchadosPerdidos;

public class IndexModel : AppBasePageModel
{
    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public IEnumerable<AchadoPerdido> Itens { get; set; } = [];

    [BindProperty]
    public AchadoPerdido NovoItem { get; set; } = new();

    [BindProperty]
    public IFormFile? Foto { get; set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (UsuarioId == Guid.Empty)
            return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login, new { returnUrl = ModuloRotas.Cidadania.AchadosPerdidos });

        Itens = await _mediator.Send(new ListarAchadosPerdidosAtivosQuery());
        return Page();
    }

    public async Task<IActionResult> OnPostPublicarAsync()
    {
        if (UsuarioId == Guid.Empty)
            return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);

        byte[]? fotoBytes = null;
        string? fotoFileName = null;
        string? fotoContentType = null;

        if (Foto != null)
        {
            using var ms = new MemoryStream();
            await Foto.CopyToAsync(ms);
            fotoBytes = ms.ToArray();
            fotoFileName = Foto.FileName;
            fotoContentType = Foto.ContentType;
        }

        var resultado = await _mediator.Send(new CadastrarAchadoPerdidoCommand(
            UsuarioId,
            NovoItem.Tipo,
            NovoItem.Titulo,
            NovoItem.Descricao,
            NovoItem.LocalVisto,
            NovoItem.DataEvento,
            NovoItem.ContatoInfo,
            NovoItem.EsconderContato,
            fotoBytes,
            fotoFileName,
            fotoContentType));

        if (!resultado.Sucesso)
            return ModuloRotas.JsonErro(resultado.Mensagem);

        Logger.LogInformation(
            "Novo item de Achados e Perdidos cadastrado (ID: {Id}) pelo usuário '{Email}'.",
            resultado.Dados!.Id, UsuarioEmail);

        return Partial("Components/_ItemCard", resultado.Dados);
    }
}
