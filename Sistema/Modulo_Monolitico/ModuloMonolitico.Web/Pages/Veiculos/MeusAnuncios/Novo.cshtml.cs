using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Comandos;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Pages.Shared;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Veiculos.MeusAnuncios;

public class NovoModel : AppBasePageModel
{
    private readonly IMediator _mediator;

    public NovoModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty] public string Marca { get; set; } = string.Empty;
    [BindProperty] public string Modelo { get; set; } = string.Empty;
    [BindProperty] public string? Versao { get; set; }
    [BindProperty] public int AnoFabricacao { get; set; } = DateTime.Now.Year;
    [BindProperty] public int AnoModelo { get; set; } = DateTime.Now.Year;
    [BindProperty] public int Quilometragem { get; set; }
    [BindProperty] public string Combustivel { get; set; } = string.Empty;
    [BindProperty] public string Cambio { get; set; } = string.Empty;
    [BindProperty] public string Cor { get; set; } = string.Empty;
    [BindProperty] public string? Placa { get; set; }
    [BindProperty] public string TipoVeiculo { get; set; } = "carro";
    [BindProperty] public short? NumeroPortas { get; set; }
    [BindProperty] public string Titulo { get; set; } = string.Empty;
    [BindProperty] public string? Descricao { get; set; }
    [BindProperty] public decimal Preco { get; set; }
    [BindProperty] public bool AceitaTroca { get; set; }
    [BindProperty] public string? ContatoWhatsapp { get; set; }
    [BindProperty] public IFormFileCollection? Fotos { get; set; }

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var comando = new CadastrarAnuncioVeiculoCommand(
            ContaId,
            UsuarioId,
            Marca,
            Modelo,
            Versao,
            AnoFabricacao,
            AnoModelo,
            Quilometragem,
            Combustivel,
            Cambio,
            Cor,
            Placa,
            TipoVeiculo,
            NumeroPortas,
            Titulo,
            Descricao,
            Preco,
            AceitaTroca,
            ContatoWhatsapp
        );

        var resultado = await _mediator.Send(comando);

        if (!resultado.Sucesso)
        {
            NotificarErro(resultado.Mensagem);
            return Page();
        }

        var anuncioId = resultado.Dados;

        if (Fotos is { Count: > 0 })
        {
            foreach (var foto in Fotos)
            {
                if (foto.Length == 0) continue;

                using var ms = new System.IO.MemoryStream();
                await foto.CopyToAsync(ms);

                var cmdFoto = new CadastrarFotoAnuncioVeiculoCommand(
                    anuncioId,
                    UsuarioId,
                    ms.ToArray(),
                    foto.FileName,
                    foto.ContentType
                );
                await _mediator.Send(cmdFoto);
            }
        }

        NotificarSucesso(MensagensSistema.Geral.Sucesso);
        return Redirect($"{ModuloRotas.Veiculos.Detalhes}?id={anuncioId}");
    }
}
