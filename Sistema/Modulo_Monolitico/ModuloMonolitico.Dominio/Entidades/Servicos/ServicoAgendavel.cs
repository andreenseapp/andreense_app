using System;

namespace ModuloMonolitico.Dominio.Entidades.Servicos;

public class ServicoAgendavel
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public Guid ContaId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal PrecoVenda { get; set; }
    public int DuracaoMinutos { get; set; }
    public string? CorHexadecimal { get; set; }
    public bool Ativo { get; set; }
}
