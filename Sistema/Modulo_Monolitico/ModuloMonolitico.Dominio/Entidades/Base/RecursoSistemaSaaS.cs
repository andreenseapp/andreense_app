using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class RecursoSistemaSaaS
{
    public Guid Id { get; set; }
    public string Chave { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? Modulo { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
}

