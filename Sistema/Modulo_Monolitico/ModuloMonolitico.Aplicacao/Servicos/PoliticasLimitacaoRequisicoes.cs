namespace ModuloMonolitico.Aplicacao.Servicos;

public static class PoliticasLimitacaoRequisicoes
{
    public static readonly PoliticaLimitacaoRequisicoes Login = new()
    {
        Nome = "login",
        Provedor = null,
        MaximoPorEmailIp = 5,
        MaximoPorIp = 20,
        Modo = ModoLimitacaoRequisicoes.Falhas
    };

    public static readonly PoliticaLimitacaoRequisicoes RecuperacaoSolicitacao = new()
    {
        Nome = "recuperacao_solicitacao",
        Provedor = "recuperacao",
        MaximoPorEmailIp = 3,
        MaximoPorIp = 10,
        Modo = ModoLimitacaoRequisicoes.Solicitacoes
    };

    public static readonly PoliticaLimitacaoRequisicoes RecuperacaoValidacao = new()
    {
        Nome = "recuperacao_validacao",
        Provedor = "recuperacao_validacao",
        MaximoPorEmailIp = 5,
        MaximoPorIp = 15,
        Modo = ModoLimitacaoRequisicoes.Falhas
    };
}
