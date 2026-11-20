using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Curriculos.DTOs;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Pages.Shared;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.MeuCurriculo;

public class IndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;

    protected override bool ExigeEmpresaAtiva => false;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public CurriculoCompletoDto? MeuCurriculo { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        MeuCurriculo = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        return Page();
    }

    public async Task<IActionResult> OnPostAlterarVisibilidadeAsync([FromForm] bool visivel)
    {
        var resultado = await _mediator.Send(new AlterarVisibilidadeCurriculoCommand(ContaId, UsuarioId, visivel));

        if (resultado.Sucesso)
            NotificarSucesso(MensagensSistema.Geral.Sucesso);
        else
            NotificarErro(resultado.Mensagem);

        return RedirectToPage();
    }
}
