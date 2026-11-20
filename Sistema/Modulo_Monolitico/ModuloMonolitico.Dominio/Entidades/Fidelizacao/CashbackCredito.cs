using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class CashbackCredito
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid? RegraId { get; set; }
    public decimal ValorInicial { get; set; }
    public decimal ValorUtilizado { get; set; }
    public string? Descricao { get; set; }
    public string? CodigoReferencia { get; set; }
    public string Situacao { get; set; } = "disponivel";
    public DateTime DataLiberacao { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public DateTime DataInclusao { get; set; }

    public decimal ValorDisponivel => ValorInicial - ValorUtilizado;
}

