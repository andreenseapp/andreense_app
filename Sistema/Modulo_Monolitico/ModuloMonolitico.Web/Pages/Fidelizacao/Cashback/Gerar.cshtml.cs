using System.ComponentModel.DataAnnotations;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;
using Net.Codecrete.QrCodeGenerator;
using ModuloMonolitico.Dominio.Configuracoes;

namespace ModuloMonolitico.Web.Pages.Fidelizacao.Cashback;

public class GerarModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Servicos.Cashback.Gerar";

    private readonly IMediator _mediator;

    public GerarModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    [Display(Name = "Valor da compra")]
    [Range(0.01, 9999999, ErrorMessage = "Informe um valor de compra maior que zero.")]
    public decimal ValorCompra { get; set; }



    public string? QrCodeBase64 { get; set; }

    public IEnumerable<CashbackRegra> RegrasAtivas { get; set; } = [];

    public CashbackRegra? RegraAplicavel { get; set; }



    [TempData]

    public string? MensagemSucesso { get; set; }



    [TempData]

    public string? MensagemErro { get; set; }



    public async Task OnGetAsync()

    {

        await CarregarRegraAplicavelAsync();



        if (RegraAplicavel != null)

        {

            Logger.LogInformation(

                "Tela de geração de QR cashback aberta pelo usuário '{Email}' — campanha '{Titulo}'.",

                UsuarioEmail, RegraAplicavel.Titulo);

        }

        else

        {

            Logger.LogWarning(

                "Tela de geração de QR cashback aberta pelo usuário '{Email}' sem campanha ativa.",

                UsuarioEmail);

        }

    }



    public async Task<IActionResult> OnPostAsync()

    {

        if (!EmpresaAtiva.HasValue)

            return ModuloRotas.RedirecionarSelecaoEmpresa(Request.Path + Request.QueryString);



        await CarregarRegraAplicavelAsync();



        if (RegraAplicavel == null)

        {

            Logger.LogWarning(

                "Geração de QR cashback bloqueada: usuário '{Email}' sem campanha ativa.",

                UsuarioEmail);

            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.NenhumaCampanhaAtiva;

            return Page();

        }



        var resultado = await _mediator.Send(new GerarCodigoCashbackCommand

        {

            EmpresaId = EmpresaAtiva.Value,

            ValorCompra = ValorCompra,

            Tipo = "credito"

        });



        if (resultado.Sucesso)

        {

            var payload = ChavesSistema.PayloadCashback(resultado.Dados);
            QrCodeBase64 = GerarSvgBase64(payload);



            Logger.LogInformation(

                "QR cashback gerado pelo usuÃ¡rio '{Email}' â€” valor R$ {Valor:F2}, campanha '{Titulo}'.",

                UsuarioEmail, ValorCompra, RegraAplicavel.Titulo);

        }

        else

        {

            Logger.LogWarning(

                "Falha ao gerar QR cashback para usuÃ¡rio '{Email}': {Mensagem}",

                UsuarioEmail, resultado.Mensagem);

            MensagemErro = resultado.Mensagem;

        }



        return Page();

    }



    private async Task CarregarRegraAplicavelAsync()

    {

        if (!EmpresaAtiva.HasValue)

            return;



        var regras = await _mediator.Send(new ObterRegrasAtivasCashbackPorEmpresaQuery(EmpresaAtiva.Value));

        var agora = DateTime.UtcNow;

        RegrasAtivas = regras.ToList();

        RegraAplicavel = RegrasAtivas.FirstOrDefault(r =>

            (r.DataInicioVigencia == null || r.DataInicioVigencia <= agora) &&

            (r.DataFimVigencia == null || r.DataFimVigencia >= agora));

    }



    private static string GerarSvgBase64(string content)

    {

        var qr = QrCode.EncodeText(content, QrCode.Ecc.Medium);

        var svg = qr.ToSvgString(4);

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(svg));

    }

}

