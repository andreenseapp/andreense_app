using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class Vendedor
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid? UsuarioId { get; set; }
    public Guid TipoVendedorId { get; set; }
    public string NomeFantasia { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public Guid? OperadorInclusao { get; set; }
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;
    public TipoVendedor? TipoVendedor { get; set; }
}

