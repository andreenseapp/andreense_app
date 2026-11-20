using System;
using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Entidades;

public class Produto
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public string? Codigo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? DescricaoComplementar { get; set; }
    public string TipoProduto { get; set; } = "produto";
    public string? Grupo { get; set; }
    public string? Marca { get; set; }
    public string UnidadeMedida { get; set; } = "UN";
    public string? Ncm { get; set; }
    public string? Cest { get; set; }
    public bool ControlaLote { get; set; } = false;

    public string? Observacao { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;

    public List<ProdutoVariacao> Variacoes { get; set; } = new();
}

