using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;
using ModuloMonolitico.Dominio.Curriculos.DTOs;
using ModuloMonolitico.Web.Pages.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.MeuCurriculo.Formacoes;

public class IndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;

    protected override bool ExigeEmpresaAtiva => false;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public IList<FormacaoDto> Formacoes { get; set; } = new List<FormacaoDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        var curriculo = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        Formacoes = curriculo?.Formacoes ?? new List<FormacaoDto>();
        return Page();
    }
}
