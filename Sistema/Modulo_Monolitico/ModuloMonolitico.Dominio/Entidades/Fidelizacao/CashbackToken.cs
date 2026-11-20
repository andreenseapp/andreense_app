using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class CashbackToken
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public Guid EmpresaId { get; set; }
    public decimal? ValorCompra { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Situacao { get; set; } = "disponivel";
    public DateTime DataExpiracao { get; set; }
    public DateTime DataInclusao { get; set; }

    public bool EstaExpirado => DateTime.UtcNow > DataExpiracao;
}

