using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class EstoqueMovimentacao
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid EstoqueSaldoId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public string? Motivo { get; set; }
    public string? Lote { get; set; }
    public DateTime? DataValidade { get; set; }
    public Guid? UsuarioResponsavelId { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
}

