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

namespace ModuloMonolitico.Web.Pages.MeuCurriculo.Experiencias;

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

    [BindProperty] public Guid ExperienciaId { get; set; }
    [BindProperty] public Guid CurriculoId { get; set; }
    [BindProperty] public string Empresa { get; set; } = string.Empty;
    [BindProperty] public string Cargo { get; set; } = string.Empty;
    [BindProperty] public DateOnly DataInicio { get; set; }
    [BindProperty] public DateOnly? DataFim { get; set; }
    [BindProperty] public bool EmAndamento { get; set; }
    [BindProperty] public string? Descricao { get; set; }
    [BindProperty] public string? Cidade { get; set; }

    public async Task<IActionResult> OnGetAsync([FromQuery] Guid id)
    {
        var curriculo = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        if (curriculo is null) return NotFound();

        CurriculoId = curriculo.Id;
        var exp = await _repositorio.ObterExperienciaPorIdAsync(ContaId, id);
        if (exp is null) return NotFound();

        ExperienciaId = exp.Id;
        Empresa = exp.Empresa;
        Cargo = exp.Cargo;
        DataInicio = exp.DataInicio;
        DataFim = exp.DataFim;
        EmAndamento = exp.EmAndamento;
        Descricao = exp.Descricao;
        Cidade = exp.Cidade;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var resultado = await _mediator.Send(new AtualizarExperienciaCommand(
            ExperienciaId, ContaId, CurriculoId, Empresa, Cargo,
            DataInicio, EmAndamento ? null : DataFim, EmAndamento, Descricao, Cidade));

        if (resultado.Sucesso)
        {
            NotificarSucesso(MensagensSistema.Geral.Sucesso);
            return Redirect(ModuloRotas.Curriculos.MeuCurriculo_Experiencias);
        }

        NotificarErro(resultado.Mensagem);
        return Page();
    }

    public async Task<IActionResult> OnPostDeletarAsync([FromForm] Guid experienciaId, [FromForm] Guid curriculoId)
    {
        var resultado = await _mediator.Send(new DeletarExperienciaCommand(ContaId, curriculoId, experienciaId));

        if (resultado.Sucesso)
            NotificarSucesso(MensagensSistema.Geral.Sucesso);
        else
            NotificarErro(resultado.Mensagem);

        return Redirect(ModuloRotas.Curriculos.MeuCurriculo_Experiencias);
    }
}
