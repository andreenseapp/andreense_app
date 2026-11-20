using System;
using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Entidades;

public class AreaEstoque
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;

    public List<LocalEstoque> Locais { get; set; } = new();
}

