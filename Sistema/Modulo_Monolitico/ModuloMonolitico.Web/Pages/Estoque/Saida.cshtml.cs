using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Estoque.ViewModels;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Estoque;

public class SaidaModel : AppBasePageModel
{
    protected override string? ChavePermissao => "ERP.Estoque.Saida";

    private readonly IMediator _mediator;

    public SaidaModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public FormularioSaida Formulario { get; set; } = new();

    public EstoqueSaldo? Saldo { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid saldoId, Guid empresaId)
    {
        Saldo = await _mediator.Send(new ObterSaldoEstoqueQuery(saldoId, ContaId));
        if (Saldo == null)
            return ModuloRotas.Redirecionar(ModuloRotas.Estoque.Index, new { empresaId });

        Formulario.SaldoId = saldoId;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Formulario.Quantidade <= 0)
        {
            Logger.LogWarning(
                "Validação falhou ao registrar saída de estoque (Saldo {SaldoId}) pelo usuário '{Email}'.",
                Formulario.SaldoId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Estoque.QuantidadeSaidaInvalida;
            Saldo = await _mediator.Send(new ObterSaldoEstoqueQuery(Formulario.SaldoId, ContaId));
            return Page();
        }

        try
        {
            var cmd = new RegistrarSaidaEstoqueCommand
            {
                SaldoId = Formulario.SaldoId,
                ContaId = ContaId,
                Quantidade = Formulario.Quantidade,
                Motivo = Formulario.Motivo,
                UsuarioResponsavelId = UsuarioId == Guid.Empty ? null : UsuarioId
            };

            var resultado = await _mediator.Send(cmd);

            Logger.LogInformation(
                "Saída de {Quantidade} unidade(s) do saldo {SaldoId} registrada na Empresa {EmpresaId} pelo usuário '{Email}'.",
                Formulario.Quantidade, Formulario.SaldoId, resultado.EmpresaId, UsuarioEmail);
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            return ModuloRotas.Redirecionar(ModuloRotas.Estoque.Index, new { empresaId = resultado.EmpresaId });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex,
                "Erro ao registrar saída de estoque (Saldo {SaldoId}) pelo usuário '{Email}'.",
                Formulario.SaldoId, UsuarioEmail);
            MensagemErro = ex is InvalidOperationException ? ex.Message : ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Estoque.ErroRegistrarSaida;
            Saldo = await _mediator.Send(new ObterSaldoEstoqueQuery(Formulario.SaldoId, ContaId));
            return Page();
        }
    }
}
