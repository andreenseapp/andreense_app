using ModuloMonolitico.Web.Core.Constantes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Pages.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Imobiliaria.MeusAnuncios;

public class NovoModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly IAmenidadeRepositorio _amenidadeRepositorio;
    private readonly IAnuncioRepositorio _anuncioRepositorio;

    public NovoModel(
        IMediator mediator,
        IAmenidadeRepositorio amenidadeRepositorio,
        IAnuncioRepositorio anuncioRepositorio)
    {
        _mediator = mediator;
        _amenidadeRepositorio = amenidadeRepositorio;
        _anuncioRepositorio = anuncioRepositorio;
    }

    [BindProperty] public string TipoNegocio { get; set; } = string.Empty;
    [BindProperty] public string TipoImovel { get; set; } = string.Empty;
    [BindProperty] public short Quartos { get; set; }
    [BindProperty] public short Banheiros { get; set; }
    [BindProperty] public short VagasGaragem { get; set; }
    [BindProperty] public short? Andar { get; set; }
    [BindProperty] public string SituacaoObra { get; set; } = "pronto";
    [BindProperty] public string[] CodigosAmenidadeSelecionados { get; set; } = [];
    [BindProperty] public decimal? AreaTotal { get; set; }
    [BindProperty] public string? Logradouro { get; set; }
    [BindProperty] public string? Numero { get; set; }
    [BindProperty] public string? Complemento { get; set; }
    [BindProperty] public string Bairro { get; set; } = string.Empty;
    [BindProperty] public string? Cep { get; set; }
    [BindProperty] public decimal? Latitude { get; set; }
    [BindProperty] public decimal? Longitude { get; set; }
    [BindProperty] public bool MostrarLogradouro { get; set; } = true;
    [BindProperty] public bool MostrarNumero { get; set; } = true;
    [BindProperty] public decimal ValorPrincipal { get; set; }
    [BindProperty] public decimal ValorCondominio { get; set; }
    [BindProperty] public decimal ValorIptu { get; set; }
    [BindProperty] public string Titulo { get; set; } = string.Empty;
    [BindProperty] public string? Descricao { get; set; }
    [BindProperty] public string? ContatoWhatsapp { get; set; }
    [BindProperty] public IFormFileCollection? Fotos { get; set; }

    public IReadOnlyList<AmenidadeDto> CatalogoAmenidades { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        CatalogoAmenidades = await _amenidadeRepositorio.ListarTodasAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        CatalogoAmenidades = await _amenidadeRepositorio.ListarTodasAsync();

        if (!ModelState.IsValid)
            return Page();

        var comando = new CadastrarAnuncioCommand(
            ContaId,
            UsuarioId,
            TipoImovel,
            AreaTotal,
            Quartos,
            Banheiros,
            VagasGaragem,
            Andar,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            SituacaoObra,
            Logradouro,
            Numero,
            Complemento,
            Bairro,
            Cep,
            Latitude,
            Longitude,
            MostrarLogradouro,
            MostrarNumero,
            TipoNegocio,
            ValorPrincipal,
            ValorCondominio,
            ValorIptu,
            Titulo,
            Descricao,
            ContatoWhatsapp
        );

        var resultado = await _mediator.Send(comando);

        if (!resultado.Sucesso)
        {
            NotificarErro(resultado.Mensagem);
            return Page();
        }

        var anuncioId = resultado.Dados;
        var anuncio = await _anuncioRepositorio.ObterAnuncioPorIdAsync(anuncioId);
        if (anuncio is not null)
        {
            await _amenidadeRepositorio.SubstituirDoImovelAsync(
                anuncio.ImovelId,
                CodigosAmenidadeSelecionados ?? Array.Empty<string>());
        }

        if (Fotos is { Count: > 0 })
        {
            foreach (var foto in Fotos)
            {
                if (foto.Length == 0) continue;

                using var ms = new System.IO.MemoryStream();
                await foto.CopyToAsync(ms);

                var cmdFoto = new CadastrarFotoAnuncioCommand(
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
        return Redirect($"{ModuloRotas.Imoveis.Detalhes}?id={anuncioId}");
    }
}
