using ModuloMonolitico.Web.Core.Constantes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Pages.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Imobiliaria.MeusAnuncios;

public class EditarModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly IAnuncioRepositorio _repositorio;
    private readonly IAmenidadeRepositorio _amenidadeRepositorio;

    public EditarModel(
        IMediator mediator,
        IAnuncioRepositorio repositorio,
        IAmenidadeRepositorio amenidadeRepositorio)
    {
        _mediator = mediator;
        _repositorio = repositorio;
        _amenidadeRepositorio = amenidadeRepositorio;
    }

    [BindProperty(SupportsGet = true)] public Guid Id { get; set; }

    [BindProperty] public string TipoImovel { get; set; } = string.Empty;
    [BindProperty] public decimal? AreaTotal { get; set; }
    [BindProperty] public short Quartos { get; set; }
    [BindProperty] public short Banheiros { get; set; }
    [BindProperty] public short VagasGaragem { get; set; }
    [BindProperty] public short? Andar { get; set; }
    [BindProperty] public string SituacaoObra { get; set; } = "pronto";
    [BindProperty] public string[] CodigosAmenidadeSelecionados { get; set; } = [];

    [BindProperty] public string? Cep { get; set; }
    [BindProperty] public string? Logradouro { get; set; }
    [BindProperty] public string? Numero { get; set; }
    [BindProperty] public string? Complemento { get; set; }
    [BindProperty] public string Bairro { get; set; } = string.Empty;
    [BindProperty] public decimal? Latitude { get; set; }
    [BindProperty] public decimal? Longitude { get; set; }
    [BindProperty] public bool MostrarLogradouro { get; set; }
    [BindProperty] public bool MostrarNumero { get; set; }

    [BindProperty] public string TipoNegocio { get; set; } = string.Empty;
    [BindProperty] public decimal ValorPrincipal { get; set; }
    [BindProperty] public decimal ValorCondominio { get; set; }
    [BindProperty] public decimal ValorIptu { get; set; }

    [BindProperty] public string Titulo { get; set; } = string.Empty;
    [BindProperty] public string? Descricao { get; set; }
    [BindProperty] public string? ContatoWhatsapp { get; set; }

    [BindProperty] public List<IFormFile> Fotos { get; set; } = new();

    public List<AnuncioFoto> FotosExistentes { get; set; } = new();
    public IReadOnlyList<AmenidadeDto> CatalogoAmenidades { get; set; } = [];

    [BindProperty] public string FotosRemovidasIds { get; set; } = string.Empty;

    private Guid _imovelId;

    public async Task<IActionResult> OnGetAsync()
    {
        var anuncio = await _repositorio.ObterDetalheAsync(Id);
        if (anuncio == null || anuncio.UsuarioId != UsuarioId)
        {
            NotificarErro(MensagensSistema.Imobiliaria.AnuncioNaoEncontrado);
            return ModuloRotas.Redirecionar(ModuloRotas.Imobiliaria.MeusAnuncios);
        }

        TipoImovel = anuncio.TipoImovel;
        AreaTotal = anuncio.AreaTotal;
        Quartos = anuncio.Quartos;
        Banheiros = anuncio.Banheiros;
        VagasGaragem = anuncio.VagasGaragem;
        Andar = anuncio.Andar;
        SituacaoObra = string.IsNullOrWhiteSpace(anuncio.SituacaoObra) ? "pronto" : anuncio.SituacaoObra;

        Cep = anuncio.Cep;
        Logradouro = anuncio.Logradouro;
        Numero = anuncio.Numero;
        Complemento = anuncio.Complemento;
        Bairro = anuncio.Bairro;
        Latitude = anuncio.Latitude;
        Longitude = anuncio.Longitude;
        MostrarLogradouro = anuncio.MostrarLogradouro;
        MostrarNumero = anuncio.MostrarNumero;

        TipoNegocio = anuncio.TipoNegocio;
        ValorPrincipal = anuncio.ValorPrincipal;
        ValorCondominio = anuncio.ValorCondominio;
        ValorIptu = anuncio.ValorIptu;

        Titulo = anuncio.Titulo;
        Descricao = anuncio.Descricao;
        ContatoWhatsapp = anuncio.ContatoWhatsapp;

        CatalogoAmenidades = await _amenidadeRepositorio.ListarTodasAsync();
        CodigosAmenidadeSelecionados = anuncio.Amenidades.Select(a => a.Codigo).ToArray();
        FotosExistentes = (await _repositorio.ListarFotosPorAnuncioAsync(Id)).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        CatalogoAmenidades = await _amenidadeRepositorio.ListarTodasAsync();

        if (!ModelState.IsValid)
        {
            NotificarErro(MensagensSistema.Imobiliaria.VerifiqueDados);
            return Page();
        }

        var detalhe = await _repositorio.ObterDetalheAsync(Id);
        if (detalhe is null || detalhe.UsuarioId != UsuarioId)
        {
            NotificarErro(MensagensSistema.Imobiliaria.AnuncioNaoEncontrado);
            return ModuloRotas.Redirecionar(ModuloRotas.Imobiliaria.MeusAnuncios);
        }

        _imovelId = detalhe.ImovelId;

        var comando = new AtualizarAnuncioCommand(
            Id,
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

        await _amenidadeRepositorio.SubstituirDoImovelAsync(
            _imovelId,
            CodigosAmenidadeSelecionados ?? Array.Empty<string>());

        if (Fotos is { Count: > 0 })
        {
            foreach (var foto in Fotos)
            {
                if (foto.Length == 0) continue;

                using var ms = new System.IO.MemoryStream();
                await foto.CopyToAsync(ms);

                var cmdFoto = new CadastrarFotoAnuncioCommand(
                    Id,
                    UsuarioId,
                    ms.ToArray(),
                    foto.FileName,
                    foto.ContentType
                );
                await _mediator.Send(cmdFoto);
            }
        }

        if (!string.IsNullOrEmpty(FotosRemovidasIds))
        {
            var idsRemover = FotosRemovidasIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var idStr in idsRemover)
            {
                if (Guid.TryParse(idStr, out var fotoId))
                {
                    await _mediator.Send(new DeletarFisicoFotoAnuncioCommand(fotoId, UsuarioId));
                }
            }
        }

        NotificarSucesso(MensagensSistema.Geral.Sucesso);
        return Redirect($"{ModuloRotas.Imoveis.Detalhes}?id={Id}");
    }
}
