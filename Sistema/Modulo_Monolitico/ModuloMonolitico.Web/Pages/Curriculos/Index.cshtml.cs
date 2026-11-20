using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Curriculos.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Curriculos;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private Guid _contaId;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public ResultadoPaginado<CurriculoResumoDto> Resultado { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public FiltroCurriculo Filtro { get; set; } = new();

    public bool FiltrosAtivos =>
        !string.IsNullOrWhiteSpace(Filtro.AreaAtuacao) ||
        !string.IsNullOrWhiteSpace(Filtro.CargoDesejado) ||
        !string.IsNullOrWhiteSpace(Filtro.Nivel) ||
        !string.IsNullOrWhiteSpace(Filtro.Modalidade) ||
        !string.IsNullOrWhiteSpace(Filtro.Disponibilidade) ||
        !string.IsNullOrWhiteSpace(Filtro.Idioma) ||
        Filtro.Pcd.HasValue ||
        Filtro.AceitaViagem.HasValue ||
        Filtro.AceitaMudanca.HasValue ||
        Filtro.PretensaoMinima.HasValue ||
        Filtro.PretensaoMaxima.HasValue;

    public string QueryStringFiltros
    {
        get
        {
            var partes = new List<string>();
            if (!string.IsNullOrWhiteSpace(Filtro.AreaAtuacao)) partes.Add($"&Filtro.AreaAtuacao={Uri.EscapeDataString(Filtro.AreaAtuacao)}");
            if (!string.IsNullOrWhiteSpace(Filtro.CargoDesejado)) partes.Add($"&Filtro.CargoDesejado={Uri.EscapeDataString(Filtro.CargoDesejado)}");
            if (!string.IsNullOrWhiteSpace(Filtro.Nivel)) partes.Add($"&Filtro.Nivel={Uri.EscapeDataString(Filtro.Nivel)}");
            if (!string.IsNullOrWhiteSpace(Filtro.Modalidade)) partes.Add($"&Filtro.Modalidade={Uri.EscapeDataString(Filtro.Modalidade)}");
            if (!string.IsNullOrWhiteSpace(Filtro.Disponibilidade)) partes.Add($"&Filtro.Disponibilidade={Uri.EscapeDataString(Filtro.Disponibilidade)}");
            if (!string.IsNullOrWhiteSpace(Filtro.Idioma)) partes.Add($"&Filtro.Idioma={Uri.EscapeDataString(Filtro.Idioma)}");
            if (Filtro.Pcd.HasValue) partes.Add($"&Filtro.Pcd={Filtro.Pcd}");
            if (Filtro.AceitaViagem.HasValue) partes.Add($"&Filtro.AceitaViagem={Filtro.AceitaViagem}");
            if (Filtro.AceitaMudanca.HasValue) partes.Add($"&Filtro.AceitaMudanca={Filtro.AceitaMudanca}");
            if (Filtro.PretensaoMinima.HasValue) partes.Add($"&Filtro.PretensaoMinima={Filtro.PretensaoMinima}");
            if (Filtro.PretensaoMaxima.HasValue) partes.Add($"&Filtro.PretensaoMaxima={Filtro.PretensaoMaxima}");
            return string.Concat(partes);
        }
    }

    public async Task<IActionResult> OnGetAsync([FromQuery] int pagina = 1)
    {
        if (!Guid.TryParse(User.FindFirst("conta_id")?.Value, out _contaId))
            _contaId = Guid.Empty;

        Filtro.Pagina = pagina;
        Resultado = await _mediator.Send(new ListarCurriculosQuery(_contaId, Filtro));
        return Page();
    }
}
