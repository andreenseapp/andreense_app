using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class CashbackUtilizacao
{
    public Guid Id { get; set; }
    public Guid CreditoId { get; set; }
    public decimal ValorUtilizado { get; set; }
    public Guid? VendaId { get; set; }
    public DateTime DataInclusao { get; set; }
}

