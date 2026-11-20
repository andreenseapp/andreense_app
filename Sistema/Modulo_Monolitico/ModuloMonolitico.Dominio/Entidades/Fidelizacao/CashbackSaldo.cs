using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class CashbackSaldo
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid UsuarioId { get; set; }
    public decimal SaldoDisponivel { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime DataUltimaAtualizacao { get; set; }
}

