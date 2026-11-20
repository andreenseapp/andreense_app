using MediatR;

using Microsoft.AspNetCore.Mvc;

using ModuloMonolitico.Aplicacao.CasosDeUso;


using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades;

using ModuloMonolitico.Web.Pages.Shared;

using Net.Codecrete.QrCodeGenerator;

using System.Text;



namespace ModuloMonolitico.Web.Pages.Fidelizacao.Fidelidade;



public class ValidarModel : AppBasePageModel

{

    protected override string? ChavePermissao => "Servicos.Fidelidade.Validar";



    private readonly IMediator _mediator;



    public ValidarModel(IMediator mediator) => _mediator = mediator;



    public string? QrCodeBase64 { get; set; }

    public IEnumerable<FidelidadeCartao> CartoesAtivos { get; set; } = [];

    public FidelidadeCartao? CartaoAplicavel { get; set; }



    [TempData]

    public string? MensagemSucesso { get; set; }



    [TempData]

    public string? MensagemErro { get; set; }



    public async Task OnGetAsync()

    {

        await CarregarCartaoAplicavelAsync();



        if (CartaoAplicavel != null)

        {

            Logger.LogInformation(

                "Tela de validação de visita aberta pelo usuário '{Email}' — cartão '{Titulo}'.",

                UsuarioEmail, CartaoAplicavel.Titulo);

        }

        else

        {

            Logger.LogWarning(

                "Tela de validação de visita aberta pelo usuário '{Email}' sem cartão ativo.",

                UsuarioEmail);

        }

    }



    public async Task<IActionResult> OnPostAsync()

    {

        if (!EmpresaAtiva.HasValue)

            return ModuloRotas.RedirecionarSelecaoEmpresa(Request.Path + Request.QueryString);



        await CarregarCartaoAplicavelAsync();



        if (CartaoAplicavel == null)

        {

            Logger.LogWarning(

                "Geração de QR fidelidade bloqueada: usuário '{Email}' sem cartão ativo.",

                UsuarioEmail);

            ModelState.AddModelError(string.Empty, "Nenhum cartão fidelidade ativo.");

            return Page();

        }



        var comando = new GerarCodigoFidelidadeCommand
        {
            CartaoId = CartaoAplicavel.Id,
            EmpresaId = EmpresaAtiva.Value
        };

        var resultado = await _mediator.Send(comando);

        if (resultado.Sucesso)

        {

            var payload = ChavesSistema.PayloadFidelidade(resultado.Dados);
            QrCodeBase64 = GerarSvgBase64(payload);



            Logger.LogInformation(

                "QR de visita fidelidade gerado pelo usuário '{Email}' — cartão '{Titulo}'.",

                UsuarioEmail, CartaoAplicavel.Titulo);

        }

        else

        {

            Logger.LogWarning(

                "Falha ao gerar QR fidelidade para usuÃ¡rio '{Email}': {Mensagem}",

                UsuarioEmail, resultado.Mensagem);

            ModelState.AddModelError(string.Empty, resultado.Mensagem ?? "Não foi possível gerar o código.");

        }



        return Page();

    }



    private async Task CarregarCartaoAplicavelAsync()

    {

        if (!EmpresaAtiva.HasValue)

            return;



        var cartoes = await _mediator.Send(new ListarCartoesFidelidadePorEmpresaQuery(EmpresaAtiva.Value));

        var agora = DateTime.UtcNow;

        CartoesAtivos = cartoes.Itens.Where(c => c.Situacao == "ativo").ToList();

        CartaoAplicavel = CartoesAtivos.FirstOrDefault(c =>

            (c.DataInicioVigencia == null || c.DataInicioVigencia <= agora) &&

            (c.DataFimVigencia == null || c.DataFimVigencia >= agora))

            ?? CartoesAtivos.FirstOrDefault();

    }



    private static string GerarSvgBase64(string content)

    {

        var qr = QrCode.EncodeText(content, QrCode.Ecc.Medium);

        var svg = qr.ToSvgString(4);

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(svg));

    }

}

