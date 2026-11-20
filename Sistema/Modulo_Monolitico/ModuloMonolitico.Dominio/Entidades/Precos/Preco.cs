using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class Preco
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid ProdutoVariacaoId { get; set; }
    public Guid ContextoPrecoId { get; set; }
    public decimal Valor { get; set; }
    public decimal PercentualComissao { get; set; } = 0;
    public bool Ativo { get; set; } = true;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;
    public string? ContextoNome { get; set; }
}

