using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class LocalEstoque
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid AreaEstoqueId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CodigoVisual { get; set; }
    public string Tipo { get; set; } = "pulmao";
    public int PrioridadeSaida { get; set; } = 0;
    public int Ordem { get; set; } = 0;
    public bool Ativo { get; set; } = true;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;
    public string? AreaNome { get; set; }
}

