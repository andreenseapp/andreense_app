using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Curriculos.DTOs;
using ModuloMonolitico.Web.Pages.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.MeuCurriculo;

public class IdiomasModel : AppBasePageModel
{
    private readonly IMediator _mediator;

    protected override bool ExigeEmpresaAtiva => false;

    public IdiomasModel(IMediator mediator) => _mediator = mediator;

    public Guid CurriculoId { get; set; }
    public IList<IdiomaDto> Idiomas { get; set; } = new List<IdiomaDto>();

    [BindProperty] public string IdiomasJson { get; set; } = "[]";

    public async Task<IActionResult> OnGetAsync()
    {
        var curriculo = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        CurriculoId = curriculo?.Id ?? Guid.Empty;
        Idiomas = curriculo?.Idiomas ?? new List<IdiomaDto>();
        IdiomasJson = JsonSerializer.Serialize(Idiomas.Select(i => new { i.Idioma, i.Nivel }));
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var curriculo = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));
        var curriculoId = curriculo?.Id ?? await ObterOuCriarCurriculoIdAsync();

        List<IdiomaItem> itens = new();
        try
        {
            var parsed = JsonSerializer.Deserialize<List<IdiomaJsonItem>>(IdiomasJson ?? "[]");
            if (parsed is not null)
                itens = parsed.Select(p => new IdiomaItem(p.Idioma, p.Nivel)).ToList();
        }
        catch { }

        var resultado = await _mediator.Send(new SalvarIdiomasCommand(ContaId, curriculoId, itens));

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

    private record IdiomaJsonItem(string Idioma, string Nivel);
}
