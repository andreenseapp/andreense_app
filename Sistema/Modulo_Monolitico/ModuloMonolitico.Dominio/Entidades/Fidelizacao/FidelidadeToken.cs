using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class FidelidadeToken
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public Guid CartaoId { get; set; }
    public Guid EmpresaId { get; set; }
    public string Situacao { get; set; } = "disponivel";
    public DateTime DataExpiracao { get; set; }
    public DateTime DataInclusao { get; set; }

    public bool EstaExpirado => DateTime.UtcNow > DataExpiracao;
}

