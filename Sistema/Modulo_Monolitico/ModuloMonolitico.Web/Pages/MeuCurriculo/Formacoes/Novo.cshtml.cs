using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Pages.Shared;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.MeuCurriculo.Formacoes;

public class NovoModel : AppBasePageModel
{
    private readonly IMediator _mediator;

    protected override bool ExigeEmpresaAtiva => false;

    public NovoModel(IMediator mediator) => _mediator = mediator;

    [BindProperty] public string Instituicao { get; set; } = string.Empty;
    [BindProperty] public string Curso { get; set; } = string.Empty;
    [BindProperty] public string Grau { get; set; } = "graduacao";
    [BindProperty] public DateOnly DataInicio { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(-4));
    [BindProperty] public DateOnly? DataFim { get; set; }
    [BindProperty] public bool EmAndamento { get; set; }

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var curriculo = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        var curriculoId = curriculo?.Id ?? await ObterOuCriarCurriculoIdAsync();

        var resultado = await _mediator.Send(new CadastrarFormacaoCommand(
            ContaId, curriculoId, Instituicao, Curso, Grau, DataInicio,
            EmAndamento ? null : DataFim, EmAndamento));

        if (resultado.Sucesso)
        {
            NotificarSucesso(MensagensSistema.Geral.Sucesso);
            return Redirect(ModuloRotas.Curriculos.MeuCurriculo_Formacoes);
        }

        NotificarErro(resultado.Mensagem);
        return Page();
    }

    private async Task<Guid> ObterOuCriarCurriculoIdAsync()
    {
        await _mediator.Send(new AlterarVisibilidadeCurriculoCommand(ContaId, UsuarioId, false));
        var c = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        return c?.Id ?? Guid.Empty;
    }
}
