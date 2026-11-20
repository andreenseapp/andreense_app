using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class ContextoPreco
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool EhPadrao { get; set; } = false;
    public bool Ativo { get; set; } = true;
    public int Ordem { get; set; } = 0;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;
}

