namespace ModuloMonolitico.Aplicacao.Servicos;

public sealed class PoliticaLimitacaoRequisicoes
{
    public required string Nome { get; init; }
    public string? Provedor { get; init; }
    public int MaximoPorEmailIp { get; init; }
    public int MaximoPorIp { get; init; }
    public TimeSpan Janela { get; init; } = TimeSpan.FromMinutes(15);
    public ModoLimitacaoRequisicoes Modo { get; init; }
}
