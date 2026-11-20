using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Web.Pages.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.MeuCurriculo;

public class HabilidadesModel : AppBasePageModel
{
    private readonly IMediator _mediator;

    protected override bool ExigeEmpresaAtiva => false;

    public HabilidadesModel(IMediator mediator) => _mediator = mediator;

    public Guid CurriculoId { get; set; }
    public IList<string> Habilidades { get; set; } = new List<string>();

    [BindProperty] public string HabilidadesJson { get; set; } = "[]";

    public async Task<IActionResult> OnGetAsync()
    {
        var curriculo = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        CurriculoId = curriculo?.Id ?? Guid.Empty;
        Habilidades = curriculo?.Habilidades ?? new List<string>();
        HabilidadesJson = System.Text.Json.JsonSerializer.Serialize(Habilidades);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var curriculo = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        var curriculoId = curriculo?.Id ?? await ObterOuCriarCurriculoIdAsync();

        List<string> habilidades = new();
        try
        {
            habilidades = System.Text.Json.JsonSerializer.Deserialize<List<string>>(HabilidadesJson ?? "[]") ?? new();
        }
        catch { }

        var resultado = await _mediator.Send(new SalvarHabilidadesCommand(ContaId, curriculoId, habilidades));

        if (resultado.Sucesso)
            NotificarSucesso(MensagensSistema.Geral.Sucesso);
        else
            NotificarErro(resultado.Mensagem);

        return RedirectToPage();
    }

    private async Task<Guid> ObterOuCriarCurriculoIdAsync()
    {
        await _mediator.Send(new AlterarVisibilidadeCurriculoCommand(ContaId, UsuarioId, false));
        var c = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        return c?.Id ?? Guid.Empty;
    }
}
