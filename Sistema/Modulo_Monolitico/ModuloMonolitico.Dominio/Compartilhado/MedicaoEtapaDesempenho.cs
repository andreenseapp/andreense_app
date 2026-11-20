namespace ModuloMonolitico.Dominio.Compartilhado;

public class MedicaoEtapaDesempenho
{
    public string Nome { get; set; } = string.Empty;
    public long DuracaoMs { get; set; }
    public int Ordem { get; set; }
}
