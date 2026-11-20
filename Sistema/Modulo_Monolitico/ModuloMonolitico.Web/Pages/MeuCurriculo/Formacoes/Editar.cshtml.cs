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

namespace ModuloMonolitico.Web.Pages.MeuCurriculo.Formacoes;

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

    [BindProperty] public Guid FormacaoId { get; set; }
    [BindProperty] public Guid CurriculoId { get; set; }
    [BindProperty] public string Instituicao { get; set; } = string.Empty;
    [BindProperty] public string Curso { get; set; } = string.Empty;
    [BindProperty] public string Grau { get; set; } = "graduacao";
    [BindProperty] public DateOnly DataInicio { get; set; }
    [BindProperty] public DateOnly? DataFim { get; set; }
    [BindProperty] public bool EmAndamento { get; set; }

    public async Task<IActionResult> OnGetAsync([FromQuery] Guid id)
    {
        var curriculo = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        if (curriculo is null) return NotFound();
        CurriculoId = curriculo.Id;

        var formacao = await _repositorio.ObterFormacaoPorIdAsync(ContaId, id);
        if (formacao is null) return NotFound();

        FormacaoId = formacao.Id;
        Instituicao = formacao.Instituicao;
        Curso = formacao.Curso;
        Grau = formacao.Grau;
        DataInicio = formacao.DataInicio;
        DataFim = formacao.DataFim;
        EmAndamento = formacao.EmAndamento;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var resultado = await _mediator.Send(new AtualizarFormacaoCommand(
            FormacaoId, ContaId, CurriculoId, Instituicao, Curso, Grau,
            DataInicio, EmAndamento ? null : DataFim, EmAndamento));

        if (resultado.Sucesso)
        {
            NotificarSucesso(MensagensSistema.Geral.Sucesso);
            return Redirect(ModuloRotas.Curriculos.MeuCurriculo_Formacoes);
        }

        NotificarErro(resultado.Mensagem);
        return Page();
    }

    public async Task<IActionResult> OnPostDeletarAsync([FromForm] Guid formacaoId, [FromForm] Guid curriculoId)
    {
        var resultado = await _mediator.Send(new DeletarFormacaoCommand(ContaId, curriculoId, formacaoId));

        if (resultado.Sucesso)
            NotificarSucesso(MensagensSistema.Geral.Sucesso);
        else
            NotificarErro(resultado.Mensagem);

        return Redirect(ModuloRotas.Curriculos.MeuCurriculo_Formacoes);
    }
}
