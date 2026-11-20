using System;

namespace ModuloMonolitico.Dominio.Entidades.Imobiliaria;

public class Imovel
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid UsuarioId { get; set; }
    public string TipoImovel { get; set; } = string.Empty;
    public decimal? AreaTotal { get; set; }
    public short Quartos { get; set; }
    public short Banheiros { get; set; }
    public short VagasGaragem { get; set; }
    public short? Andar { get; set; }
    public short? TotalAndares { get; set; }
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
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = "Santo André";
    public string? Cep { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;
}
