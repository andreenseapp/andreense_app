using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Pages.Shared;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.MeuCurriculo.Cursos;

public class NovoModel : AppBasePageModel
{
    private readonly IMediator _mediator;

    protected override bool ExigeEmpresaAtiva => false;

    public NovoModel(IMediator mediator) => _mediator = mediator;

    [BindProperty] public string Nome { get; set; } = string.Empty;
    [BindProperty] public string? Instituicao { get; set; }
    [BindProperty] public int? CargaHoraria { get; set; }
    [BindProperty] public short? AnoConclusao { get; set; }

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var curriculo = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        var curriculoId = curriculo?.Id ?? await ObterOuCriarCurriculoIdAsync();

        var resultado = await _mediator.Send(new CadastrarCursoCommand(
            ContaId, curriculoId, Nome, Instituicao, CargaHoraria, AnoConclusao));

        if (resultado.Sucesso)
        {
            NotificarSucesso(MensagensSistema.Geral.Sucesso);
            return Redirect(ModuloRotas.Curriculos.MeuCurriculo_Cursos);
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
