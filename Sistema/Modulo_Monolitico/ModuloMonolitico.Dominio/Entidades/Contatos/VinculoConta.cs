namespace ModuloMonolitico.Dominio.Entidades;

public class VinculoConta
{
    public Guid Id { get; set; }
    public Guid ContaAId { get; set; }
    public Guid ContaBId { get; set; }
    public string Tipo { get; set; } = "confianca";
    public string Situacao { get; set; } = "ativo";
    public Guid? SolicitacaoId { get; set; }
    public DateTime DataAceite { get; set; }
    public DateTime DataInclusao { get; set; }
    public string? NomeContaParceira { get; set; }
    public string? EmailContaParceira { get; set; }
}
