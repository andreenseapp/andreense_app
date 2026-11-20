using System;

namespace ModuloMonolitico.Dominio.Entidades.Veiculos;

public class AnuncioVeiculo
{
    public Guid Id { get; set; }
    public Guid VeiculoId { get; set; }
    public Guid ContaId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public bool AceitaTroca { get; set; }
    public string? ContatoWhatsapp { get; set; }
    public string Situacao { get; set; } = "ativo";
    public bool EhDestaque { get; set; }
    public string? CodigoPublico { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;
}
