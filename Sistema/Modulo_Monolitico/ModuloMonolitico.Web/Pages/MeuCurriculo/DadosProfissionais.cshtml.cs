using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;
using ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Curriculos.DTOs;
using ModuloMonolitico.Web.Pages.Shared;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.MeuCurriculo;

public class DadosProfissionaisModel : AppBasePageModel
{
    private readonly IMediator _mediator;

    protected override bool ExigeEmpresaAtiva => false;

    public DadosProfissionaisModel(IMediator mediator) => _mediator = mediator;

    [BindProperty] public string CargoDesejado { get; set; } = string.Empty;
    [BindProperty] public string AreaAtuacao { get; set; } = string.Empty;
    [BindProperty] public string Nivel { get; set; } = "pleno";
    [BindProperty] public string Modalidade { get; set; } = "clt";
    [BindProperty] public decimal? PretensaoSalarial { get; set; }
    [BindProperty] public string Disponibilidade { get; set; } = "imediata";
    [BindProperty] public bool AceitaViagem { get; set; }
    [BindProperty] public bool AceitaMudanca { get; set; }
    [BindProperty] public string? ResumoProfissional { get; set; }
    [BindProperty] public string? CnhCategorias { get; set; }
    [BindProperty] public bool Pcd { get; set; }
    [BindProperty] public string? TipoDeficiencia { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var curriculo = await _mediator.Send(new ObterMeuCurriculoQuery(ContaId, UsuarioId));

        if (curriculo is not null)
        {
            CargoDesejado = curriculo.CargoDesejado;
            AreaAtuacao = curriculo.AreaAtuacao;
            Nivel = curriculo.Nivel;
            Modalidade = curriculo.Modalidade;
            PretensaoSalarial = curriculo.PretensaoSalarial;
            Disponibilidade = curriculo.Disponibilidade;
            AceitaViagem = curriculo.AceitaViagem;
            AceitaMudanca = curriculo.AceitaMudanca;
            ResumoProfissional = curriculo.ResumoProfissional;
            CnhCategorias = curriculo.CnhCategorias;
            Pcd = curriculo.Pcd;
            TipoDeficiencia = curriculo.TipoDeficiencia;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var resultado = await _mediator.Send(new SalvarCurriculoCommand(
            ContaId, UsuarioId,
            CargoDesejado, AreaAtuacao, Nivel, Modalidade, PretensaoSalarial, Disponibilidade,
            AceitaViagem, AceitaMudanca, ResumoProfissional, CnhCategorias, Pcd, TipoDeficiencia));

        if (resultado.Sucesso)
            NotificarSucesso(MensagensSistema.Geral.Sucesso);
        else
            NotificarErro(resultado.Mensagem);

        return RedirectToPage();
    }
}
