using System;
using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Imobiliaria.DTOs;

public class AnuncioDetalheDto
{
    public Guid AnuncioId { get; set; }
    public Guid ImovelId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string TipoNegocio { get; set; } = string.Empty;
    public string TipoImovel { get; set; } = string.Empty;
    public decimal ValorPrincipal { get; set; }
    public decimal ValorCondominio { get; set; }
    public decimal ValorIptu { get; set; }
    public decimal PacoteTotal { get; set; }
    public string? ContatoWhatsapp { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string? Cep { get; set; }
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public bool MostrarLogradouro { get; set; }
    public bool MostrarNumero { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public short Quartos { get; set; }
    public short Banheiros { get; set; }
    public short VagasGaragem { get; set; }
    public decimal? AreaTotal { get; set; }
    public short? Andar { get; set; }
    public bool TemElevador { get; set; }
    public bool TemPiscina { get; set; }
    public bool AceitaPet { get; set; }
    public bool TemChurrasqueira { get; set; }
    public bool TemSalaoFestas { get; set; }
    public bool TemQuadra { get; set; }
    public bool TemAcademia { get; set; }
    public bool TemPortaria24h { get; set; }
    public bool TemVaranda { get; set; }
    public bool TemPlayground { get; set; }
    public bool TemAguaSeparada { get; set; }
    public bool TemLuzSeparada { get; set; }
    public bool TemEntradaSeparada { get; set; }
    public bool TemCobertura { get; set; }
    public string SituacaoObra { get; set; } = "pronto";
    public string? CodigoPublico { get; set; }
    public string Situacao { get; set; } = string.Empty;
    public DateTime DataInclusao { get; set; }
    public List<string> Fotos { get; set; } = new();
    public List<AmenidadeDto> Amenidades { get; set; } = new();
    public Guid ContaId { get; set; }
    public string NomeAnunciante { get; set; } = string.Empty;
    public string? LogoAnunciante { get; set; }
    public string? CreciAnunciante { get; set; }
    public string? UrlSiteAnunciante { get; set; }
    public string? InstagramAnunciante { get; set; }
}
