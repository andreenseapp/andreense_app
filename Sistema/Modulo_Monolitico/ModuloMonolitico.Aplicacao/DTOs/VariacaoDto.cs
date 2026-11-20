namespace ModuloMonolitico.Aplicacao.DTOs;

public class VariacaoDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Descricao { get; set; } = string.Empty;
    public string? CodigoBarras { get; set; }
    public string? CodigoSku { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal Custo { get; set; }
    public decimal EstoqueAtual { get; set; }
    public decimal EstoqueMinimo { get; set; }
}
