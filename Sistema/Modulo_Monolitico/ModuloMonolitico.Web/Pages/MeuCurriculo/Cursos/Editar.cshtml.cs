using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Pages.Shared;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.MeuCurriculo.Cursos;

public class EditarModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ICurriculoRepositorio _repositorio;

    protected override bool ExigeEmpresaAtiva => false;

    public EditarModel(IMediator mediator, ICurriculoRepositorio repositorio)
    {
        _mediator = mediator;
        _repositorio = repositorio;
    }

    [BindProperty] public Guid CursoId { get; set; }
    [BindProperty] public Guid CurriculoId { get; set; }
    [BindProperty] public string Nome { get; set; } = string.Empty;
    [BindProperty] public string? Instituicao { get; set; }
    [BindProperty] public int? CargaHoraria { get; set; }
    [BindProperty] public short? AnoConclusao { get; set; }

    public async Task<IActionResult> OnGetAsync([FromQuery] Guid id)
    {
        var curriculo = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        if (curriculo is null) return NotFound();
        CurriculoId = curriculo.Id;

        var curso = await _repositorio.ObterCursoPorIdAsync(ContaId, id);
        if (curso is null) return NotFound();

        CursoId = curso.Id;
        Nome = curso.Nome;
        Instituicao = curso.Instituicao;
        CargaHoraria = curso.CargaHoraria;
        AnoConclusao = curso.AnoConclusao;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var resultado = await _mediator.Send(new AtualizarCursoCommand(
            CursoId, ContaId, CurriculoId, Nome, Instituicao, CargaHoraria, AnoConclusao));

        if (resultado.Sucesso)
        {
            NotificarSucesso(MensagensSistema.Geral.Sucesso);
            return Redirect(ModuloRotas.Curriculos.MeuCurriculo_Cursos);
        }

        NotificarErro(resultado.Mensagem);
        return Page();
    }

    public async Task<IActionResult> OnPostDeletarAsync([FromForm] Guid cursoId, [FromForm] Guid curriculoId)
    {
        var resultado = await _mediator.Send(new DeletarCursoCommand(ContaId, curriculoId, cursoId));

        if (resultado.Sucesso)
            NotificarSucesso(MensagensSistema.Geral.Sucesso);
        else
            NotificarErro(resultado.Mensagem);

        return Redirect(ModuloRotas.Curriculos.MeuCurriculo_Cursos);
    }
}
