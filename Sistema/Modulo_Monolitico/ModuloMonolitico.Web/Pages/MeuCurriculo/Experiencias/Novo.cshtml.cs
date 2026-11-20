using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Pages.Shared;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.MeuCurriculo.Experiencias;

public class NovoModel : AppBasePageModel
{
    private readonly IMediator _mediator;

    protected override bool ExigeEmpresaAtiva => false;

    public NovoModel(IMediator mediator) => _mediator = mediator;

    [BindProperty] public string Empresa { get; set; } = string.Empty;
    [BindProperty] public string Cargo { get; set; } = string.Empty;
    [BindProperty] public DateOnly DataInicio { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(-1));
    [BindProperty] public DateOnly? DataFim { get; set; }
    [BindProperty] public bool EmAndamento { get; set; }
    [BindProperty] public string? Descricao { get; set; }
    [BindProperty] public string? Cidade { get; set; }

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var curriculo = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        Guid curriculoId;

        if (curriculo is null)
            curriculoId = await _mediator.Send(new AlterarVisibilidadeCurriculoCommand(ContaId, UsuarioId, false))
                is { } ? await ObterCurriculoIdAsync() : Guid.Empty;
        else
            curriculoId = curriculo.Id;

        if (curriculoId == Guid.Empty)
        {
            NotificarErro("Não foi possível identificar o currículo.");
            return Page();
        }

        var resultado = await _mediator.Send(new CadastrarExperienciaCommand(
            ContaId, curriculoId, Empresa, Cargo, DataInicio,
            EmAndamento ? null : DataFim, EmAndamento, Descricao, Cidade));

        if (resultado.Sucesso)
        {
            NotificarSucesso(MensagensSistema.Geral.Sucesso);
            return Redirect(ModuloRotas.Curriculos.MeuCurriculo_Experiencias);
        }

        NotificarErro(resultado.Mensagem);
        return Page();
    }

    private async Task<Guid> ObterCurriculoIdAsync()
    {
        var c = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        return c?.Id ?? Guid.Empty;
    }
}
